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

	/// <summary>
	/// List of mods declared by the client using the handshake.
	/// </summary>
	public class ModList
	{
		[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1000:KeywordsMustBeSpacedCorrectly", Justification = "Stylecop documentation does not match implementation")]
		private readonly ConcurrentDictionary<string, ClientMod> idToMod = new();
		[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1000:KeywordsMustBeSpacedCorrectly", Justification = "Stylecop documentation does not match implementation")]
		private readonly ConcurrentDictionary<uint, ClientMod> netIdToMod = new();
		private readonly int length;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModList"/> class.
		/// </summary>
		/// <param name="length">The amount of mods that are expected to be declared.</param>
		public ModList(int length)
		{
			this.length = length;
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
				if (this.idToMod[mod.Id].Equals(mod))
				{
					return null;
				}

				return $"<color=\"red\">Error BHS01:</color> Mod {mod.Id} was already registered.";
			}

			if (!this.netIdToMod.TryAdd(mod.NetId, mod))
			{
				return $"<color=\"red\">Error BHS02:</color> Could not register {mod.Id} as mod {mod.Id} already used netId {mod.NetId}.";
			}

			return null;
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
		/// <para>Checks if this modlist is compatible with another modlist.</para>
		/// <para>Note that client-only mods are allowed to be in either list.</para>
		/// </summary>
		/// <param name="host">The other modlist to check.</param>
		/// <param name="reason">If failed, this is the user-displayable reason why.</param>
		/// <returns>true iff all elements of this list are in the other list.</returns>
		public bool IsCompatibleWith(ModList host, out string? reason)
		{
			foreach (ClientMod m in this.idToMod.Values)
			{
				Console.WriteLine($"testing {m.Id}");
				if (m.Side == ReactorPluginSide.ClientOnly)
				{
					Console.WriteLine("skipping");
					continue; // Client mods are okay to use on servers.
				}

				if (host.idToMod.TryGetValue(m.Id, out ClientMod? otherMod))
				{
					Console.WriteLine("version check");
					if (m.Version != otherMod.Version)
					{
						reason = $"<color=\"red\">Error BHS22:</color> Version of {m.Id} does not match: you have {m.Version}, host has {otherMod.Version}";
						return false;
					}
				}
				else
				{
					reason = $"<color=\"red\">Error BHS20:</color> Host does not have a mod called {m.Id}";
					return false;
				}
			}

			foreach (ClientMod mod in host.idToMod.Values)
			{
				Console.WriteLine($"testing {mod.Id} for host");
				if (mod.Side != ReactorPluginSide.ClientOnly && !this.idToMod.ContainsKey(mod.Id))
				{
					reason = $"<color=\"red\">Error BHS21:</color> You are missing a mod called {mod.Id}";
					return false;
				}
			}

			reason = null;
			return true;
		}
	}
}
