// <copyright file="GameEventListener.cs" company="miniduikboot">
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

namespace Boot.Handshake.Handlers
{
	using Boot.Handshake.Enums;
	using Boot.Handshake.Extensions;
	using Impostor.Api.Events;
	using Impostor.Api.Games;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Logging.Abstractions;

	/// <summary>
	/// Check if mod list is correct when players are joining.
	/// </summary>
	public class GameEventListener : IEventListener
	{
		private readonly ILogger<GameEventListener> logger;
		private readonly ModListManager listManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="GameEventListener"/> class.
		/// </summary>
		/// <param name="logger">Logger for this class.</param>
		/// <param name="listManager">List manager to provide mod lists.</param>
		public GameEventListener(ILogger<GameEventListener> logger, ModListManager listManager)
		{
			this.logger = logger;
			this.listManager = listManager;
		}

		/// <summary>
		/// Check if a player has compatible mods for this game.
		/// </summary>
		/// <param name="ev">Event containing the player connection.</param>
		[EventListener]
		public void OnGamePlayerJoining(IGamePlayerJoiningEvent ev)
		{
			var client = ev.Player.Client;
			var playerList = this.listManager.Get(client);

			if (playerList?.IsComplete() == false)
			{
				this.logger.LogError("Joining, but modlist is not complete yet!");
			}

			if (ev.Game.Host == null)
			{
				this.logger.LogInformation("Joining game with no host");
				return;
			}

			var hostList = this.listManager.Get(ev.Game.Host.Client);

			if (hostList == null)
			{
				if (playerList != null)
				{
					// The host is not modded, while the client is.
					ev.JoinResult = ErrorCode.BHS10.GetJoinResult();
				}

				// else, none of the players are modded.
			}
			else
			{
				if (playerList == null)
				{
					// The host is modded, while the client is not.
					ev.JoinResult = ErrorCode.BHS11.GetJoinResult();
				}
				else if (!playerList.IsCompatibleWith(hostList, out var reason))
				{
					// the mods are incompatible, either because the mod lists do not contain the same mods,
					// or because one or more mods are of different versions.
					ev.JoinResult = GameJoinResult.CreateCustomError(reason);
				}
			}
		}
	}
}
