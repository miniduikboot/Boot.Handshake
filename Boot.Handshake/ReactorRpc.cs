// <copyright file="ReactorRpc.cs" company="miniduikboot">
// This file is part of Boot.Handshake.
//
// Boot.Handshake is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Boot.Handshake is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Boot.Handshake. If not, see https://www.gnu.org/licenses/
// </copyright>

namespace Boot.Handshake
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Boot.Handshake.Enums;
	using Boot.Handshake.Extensions;
	using Impostor.Api.Innersloth;
	using Impostor.Api.Net;
	using Impostor.Api.Net.Custom;
	using Impostor.Api.Net.Inner;
	using Impostor.Api.Net.Messages;
	using Microsoft.Extensions.Logging;

	/// <summary>
	/// <para>Handle Reactor Rpcs.</para>
	/// <para>Every client has declared mods, which are checked in an earlier stage. When an Reactor RPC is executed, the mod network id must be remapped to that of each player.</para>
	/// </summary>
	public class ReactorRpc : ICustomRpc
	{
		private readonly ModListManager listManager;
		private readonly ILogger<ReactorRpc> logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactorRpc"/> class.
		/// </summary>
		/// <param name="listManager">Mod list manager for remapping id's.</param>
		/// <param name="logger">Logs issues.</param>
		public ReactorRpc(ModListManager listManager, ILogger<ReactorRpc> logger)
		{
			this.listManager = listManager;
			this.logger = logger;
		}

		/// <inheritdoc/>
		public byte Id => 0xFF;

		/// <inheritdoc/>
		public async ValueTask<bool> HandleRpcAsync(IInnerNetObject innerNetObject, IClientPlayer sender, IClientPlayer? target, IMessageReader reader)
		{
			var senderNetId = reader.ReadPackedUInt32();
			var modRpcId = reader.ReadPackedUInt32();
			var msgLength = reader.ReadUInt16();

			// Hazel messages also include a tag of 1 byte which is not included in the message length.
			var msgData = reader.ReadBytes(msgLength + 1);

			var senderList = this.listManager.Get(sender.Client);
			if (senderList == null)
			{
				await sender.Client.DisconnectAsync(DisconnectReason.Custom, ErrorCode.BHS30.GetClientMessage()).ConfigureAwait(false);
				return false;
			}

			var mod = senderList.MapNetId(senderNetId);
			if (mod == null || mod.Side == ReactorPluginSide.ClientOnly)
			{
				await sender.Client.DisconnectAsync(DisconnectReason.Custom, ErrorCode.BHS31.GetClientMessage(mod == null ? senderNetId : mod.Id)).ConfigureAwait(false);
				return false;
			}

			void FillRpc(IMessageWriter writer)
			{
				writer.WritePacked(modRpcId);
				writer.Write(msgLength);
				writer.Write(msgData);
			}

			if (target == null)
			{
				this.logger.LogDebug(
					"{Sender} broadcasted Reactor Custom RPC {SenderNetId}:{modRpcId}, length {Length}",
					sender.Character?.PlayerInfo.PlayerName,
					senderNetId,
					modRpcId,
					msgLength);

				// Broadcast this RPC to all players
				var tasks = new List<Task>();
				foreach (var player in innerNetObject.Game.Players)
				{
					if (player == sender)
					{
						continue; // Don't echo back RPC's to the sender
					}

					tasks.Add(this.SendRpcTo(innerNetObject, target, player, mod.Id, FillRpc));
				}

				await Task.WhenAll(tasks).ConfigureAwait(false);
			}
			else
			{
				this.logger.LogDebug(
					"{Sender} sent Reactor Custom RPC {SenderNetId}:{modRpcId} to {Target}, length{Length}",
					sender.Character?.PlayerInfo.PlayerName,
					senderNetId,
					modRpcId,
					target.Character?.PlayerInfo.PlayerName,
					msgLength);

				// Only send this to the intended player
				await this.SendRpcTo(innerNetObject, target, target, mod.Id, FillRpc).ConfigureAwait(false);
			}

			// Don't send out the original RPC
			return false;
		}

		private Task SendRpcTo(IInnerNetObject innerNetObject, IClientPlayer? target, IClientPlayer receiver, string id, Action<IMessageWriter> fillRpc)
		{
			var receiverList = this.listManager.Get(receiver.Client);
			if (receiverList == null)
			{
				return Task.CompletedTask;
			}

			var receiverMod = receiverList.MapId(id);
			if (receiverMod == null)
			{
				return Task.CompletedTask;
			}

			var game = innerNetObject.Game;

			var writer = game.StartRpc(innerNetObject.NetId, (RpcCalls)this.Id, target?.Client.Id);
			writer.WritePacked(receiverMod.NetId);
			fillRpc(writer);
			return game.FinishRpcAsync(writer, receiver.Client.Id).AsTask();
		}
	}
}
