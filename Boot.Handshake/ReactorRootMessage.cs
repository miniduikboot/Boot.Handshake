// <copyright file="ReactorRootMessage.cs" company="miniduikboot">
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
	using System.Threading.Tasks;
	using Boot.Handshake.Enums;
	using Boot.Handshake.Messages;
	using Impostor.Api.Innersloth;
	using Impostor.Api.Net;
	using Impostor.Api.Net.Custom;
	using Impostor.Api.Net.Messages;
	using Microsoft.Extensions.Logging;

	/// <summary>
	/// Handle the Reactor messages, which includes ModDeclarationMessages.
	/// </summary>
	public class ReactorRootMessage : ICustomRootMessage
	{
		private readonly ILogger logger;
		private readonly ModListManager modlists;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReactorRootMessage"/> class.
		/// </summary>
		/// <param name="logger">Logger for this class.</param>
		/// <param name="modlists">Manager of modlists.</param>
		public ReactorRootMessage(ILogger<ReactorRootMessage> logger, ModListManager modlists)
		{
			this.logger = logger;
			this.modlists = modlists;
		}

		/// <summary>
		/// Gets the Id of the Reactor handshake as declared by the Reactor source.
		/// </summary>
		public byte Id => 0xff;

		/// <inheritdoc/>
		public async ValueTask HandleMessageAsync(IClient client, IMessageReader reader, MessageType messageType)
		{
			ReactorMessageTypes tag = (ReactorMessageTypes)reader.ReadByte();
			switch (tag)
			{
				case ReactorMessageTypes.ModDeclaration:
					ModDeclarationMessage.Deserialize(reader, out var netId, out var id, out var version, out var side);
					var mod = new ClientMod(netId, id, version, side);
					this.logger.LogInformation("Mod: {@Mod}", mod);

					var modList = this.modlists.Get(client);
					if (modList == null)
					{
						await client.DisconnectAsync(DisconnectReason.Custom, "<color=\"red\">Error BHS03:</color> Cannot accept mod declarations when initial handshake was not made").ConfigureAwait(false);
						break;
					}

					var failureReason = modList.RegisterMod(mod);
					if (failureReason != null)
					{
						await client.DisconnectAsync(DisconnectReason.Custom, failureReason).ConfigureAwait(false);
					}

					break;
				default:
					this.logger.LogWarning("Unsupported ReactorRootMessage: {Tag}", tag);
					break;
			}
		}
	}
}
