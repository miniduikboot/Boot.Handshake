// <copyright file="ModList.cs" company="miniduikboot">
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
	using System.Collections.Concurrent;
	using System.Diagnostics.CodeAnalysis;
	using Boot.Handshake.Enums;
	using Boot.Handshake.Extensions;
	using Microsoft.Extensions.Logging;

	/// <summary>
	/// List of mods declared by the client using the handshake.
	/// </summary>
	public class ModList
	{
		/// <summary>
		/// Converts a mod ID string to the class. This mod ID is provided in the handshake.
		/// </summary>
		private readonly ConcurrentDictionary<string, ClientMod> idToMod = new ();

		/// <summary>
		/// Converts a netID to the class. This net ID is provided by the client.
		/// </summary>
		private readonly ConcurrentDictionary<uint, ClientMod> netIdToMod = new ();

		/// <summary>
		/// The number of mods in the list.
		/// </summary>
		private readonly int length;
		private readonly ILogger<ModList> logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModList"/> class.
		/// </summary>
		/// <param name="length">The amount of mods that are expected to be declared.</param>
		/// <param name="logger">Logger for the current class.</param>
		public ModList(int length, ILogger<ModList> logger)
		{
			this.length = length;
			this.logger = logger;
		}

		/// <summary>
		/// <para>Register a <see cref="ClientMod"/> to the list.</para>
		/// <para>This mod must have an unique name and network id compared to the mods already in this <see cref="ModList"/>.</para>
		/// </summary>
		/// <param name="mod">The client mod to register.</param>
		/// <returns>If successful, returns null. When unsucessful, returns the failure reason.</returns>
		public string? RegisterMod(ClientMod mod)
		{
			if (!this.idToMod.TryAdd(mod.Id, mod))
			{
				// Reactor currently registers every mod twice, if they're identical, ignore the second registration attempt.
				return this.idToMod[mod.Id].Equals(mod) ? null : ErrorCode.BHS01.GetClientMessage(mod.Id);
			}

			return !this.netIdToMod.TryAdd(mod.NetId, mod) ? ErrorCode.BHS02.GetClientMessage(mod.Id, mod.Id, mod.NetId) : null;
		}

		/// <summary>
		/// Check if a mod list is complete or if more mods should still be submitted.
		/// </summary>
		/// <returns>If there are more or the same amount of mods as declared in the initial handshake.</returns>
		public bool IsComplete()
		{
			return this.idToMod.Count >= this.length;
		}

		/// <summary>
		/// <para>Checks if this mod list is compatible with another mod list.</para>
		/// <para>Note that client-only mods are allowed to be in either list.</para>
		/// </summary>
		/// <param name="hostModList">The other mod list to check.</param>
		/// <param name="reason">If failed, this is the formatted error message that is to be sent to the player.</param>
		/// <returns>true if all elements of this list are in the other list.</returns>
		public bool IsCompatibleWith(ModList hostModList, out string reason)
		{
			foreach (var mod in this.idToMod.Values)
			{
				this.logger.LogDebug("Testing client \"{modId}\" for compatibility.", mod.Id);

				if (mod.Side == ReactorPluginSide.ClientOnly)
				{
					this.logger.LogDebug("Skipping client only mod \"{name}\".", mod.Id);
					continue; // Client mods are okay to use on servers.
				}

				if (hostModList.idToMod.TryGetValue(mod.Id, out var otherMod))
				{
					if (mod.Version == otherMod.Version)
					{
						continue;
					}

					this.logger.LogDebug(
						"BHS22: Version check for {modId} failed ({modIdVersion} is not {otherModVersion}).", mod.Id, mod.Version, otherMod.Version);

					reason = ErrorCode.BSH22.GetClientMessage(mod.Id, mod.Version, otherMod.Version);
					return false;
				}

				// the client has a mod, but the host does not have it.
				this.logger.LogDebug("BHS20: Host does not have a mod called {modId}", mod.Id);
				reason = ErrorCode.BHS20.GetClientMessage(mod.Id);
				return false;
			}

			foreach (var hostMod in hostModList.idToMod.Values)
			{
				this.logger.LogDebug("Testing host {modId} for compatibility.", hostMod.Id);

				if (hostMod.Side == ReactorPluginSide.ClientOnly || this.idToMod.ContainsKey(hostMod.Id))
				{
					continue;
				}

				// the host has a mod, but the client does not.
				this.logger.LogDebug("BHS21: missing {modId}", hostMod.Id);
				reason = ErrorCode.BHS21.GetClientMessage(hostMod.Id);
				return false;
			}

			reason = string.Empty;
			return true;
		}
	}
}
