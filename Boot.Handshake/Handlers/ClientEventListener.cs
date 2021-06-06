// <copyright file="ClientEventListener.cs" company="miniduikboot">
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
	using System.Collections.Concurrent;
	using Boot.Handshake.Messages;
	using Impostor.Api.Events;
	using Impostor.Api.Events.Client;
	using Impostor.Api.Net;
	using Impostor.Api.Net.Messages;
	using Impostor.Api.Utils;
	using Microsoft.Extensions.Logging;

	/// <summary>
	/// Deals with Client Events, including the initial handshake.
	/// </summary>
	public class ClientEventListener : IEventListener
	{
		private readonly IServerEnvironment environment;
		private readonly ILogger logger;
		private readonly IMessageWriterProvider writerProvider;
		private readonly ModListManager listManager;
		private readonly ModListFactory listFactory;

		private readonly ConcurrentDictionary<IHazelConnection, int> announcedMods = new ();

		/// <summary>
		/// Initializes a new instance of the <see cref="ClientEventListener"/> class.
		/// </summary>
		/// <param name="environment">Get the server environment, which has the version.</param>
		/// <param name="logger">Register a Logger.</param>
		/// <param name="writerProvider">Get the MessageWriter pool.</param>
		/// <param name="listManager">Get the Mod List Manager.</param>
		/// <param name="listFactory">Get the Mod List Factory.</param>
		public ClientEventListener(IServerEnvironment environment, ILogger<ClientEventListener> logger, IMessageWriterProvider writerProvider, ModListManager listManager, ModListFactory listFactory)
		{
			this.environment = environment;
			this.logger = logger;
			this.writerProvider = writerProvider;
			this.listManager = listManager;
			this.listFactory = listFactory;
		}

		/// <summary>
		/// Check for a reactor handshake and if present, give server response.
		/// </summary>
		/// <param name="ev">Event containing the client handshake.</param>
		[EventListener]
		public async void OnClientConnection(IClientConnectionEvent ev)
		{
			var reader = ev.HandshakeData;

			// Reactor handshake is attached to the initial handshake, check if there is data after the vanilla handshake.
			if (reader.Length - reader.Position == 0)
			{
				this.logger.LogInformation("No Reactor Handshake detected");
				return;
			}

			ModdedHandshakeC2S.Deserialize(reader, out byte protocolVersion, out int modCount);
			this.logger.LogInformation("Reactor Protocol version: {Version}, Mod Count: {ModCount}", protocolVersion, modCount);

			if (protocolVersion != 1)
			{
				return; // We don't know what to do with newer versions
			}

			_ = this.announcedMods.TryAdd(ev.Connection, modCount);

			var writer = this.writerProvider.Get(MessageType.Reliable);
			writer.StartMessage(0xff);

			// TODO send plugin declarations.
			ModdedHandshakeS2C.Serialize(writer, "Impostor", this.environment.Version, 0);
			writer.EndMessage();
			await ev.Connection.SendAsync(writer).ConfigureAwait(false);
		}

		/// <summary>
		/// Link handshake data from connection to the correct client.
		/// </summary>
		/// <param name="ev">Event containing the client.</param>
		[EventListener]
		public void OnClientConnected(IClientConnectedEvent ev)
		{
			if (this.announcedMods.TryRemove(ev.Connection, out var modCount))
			{
				_ = this.listManager.Add(ev.Client, this.listFactory.Create(modCount));
			}
		}
	}
}
