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
	using Impostor.Api.Events;
	using Impostor.Api.Games;
	using Microsoft.Extensions.Logging;

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
			var myList = this.listManager.Get(client);

			if (myList?.IsComplete() == false)
			{
				this.logger.LogError("Joining, but modlist is not complete yet!");
			}

			if (ev.Game.Host == null)
			{
				this.logger.LogInformation("Joining game with no host");
				return;
			}

			var hostList = this.listManager.Get(ev.Game.Host.Client);

			// Check if host and players either both have mods or both don't have mods
			if (hostList == null)
			{
				if (myList != null)
				{
					ev.JoinResult = GameJoinResult.CreateCustomError("<color=\"red\">Error BHS10:</color> Sorry, you cannot join an unmodded lobby with mods.");
				}
			}
			else
			{
				if (myList == null)
				{
					ev.JoinResult = GameJoinResult.CreateCustomError("<color=\"red\">Error BHS11:</color> Sorry, you cannot join a modded lobby without mods.");
				}
				else if (!myList.IsCompatibleWith(hostList, out var reason))
				{
					ev.JoinResult = GameJoinResult.CreateCustomError(reason!);
				}
			}
		}
	}
}
