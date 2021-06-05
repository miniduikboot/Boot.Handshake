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
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// List of mods declared by the client using the handshake.
	/// </summary>
	public class ModList
	{
		[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1000:KeywordsMustBeSpacedCorrectly", Justification = "Stylecop documentation does not match implementation")]
		private readonly Dictionary<string, ClientMod> idToMod = new();
		[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1000:KeywordsMustBeSpacedCorrectly", Justification = "Stylecop documentation does not match implementation")]
		private readonly Dictionary<uint, ClientMod> netIdToMod = new();

		/// <summary>
		/// <para>Register a <see cref="ClientMod"/> to the list.</para>
		/// <para>This mod must have an unique name and network id compared to the mods already in this <see cref="ModList"/>.</para>
		/// </summary>
		/// <param name="mod">The client mod to register.</param>
		/// <exception cref="HandshakeException">when the mod could not be registered.</exception>
		public void RegisterMod(ClientMod mod)
		{
			if (!this.idToMod.TryAdd(mod.Id, mod))
			{
				// Reactor currently registers every mod twice, if they're identical, ignore the second registration attempt.
				if (this.idToMod[mod.Id].Equals(mod))
				{
					return;
				}

				throw new HandshakeException($"Mod {mod.Id} was already registered.");
			}

			if (!this.netIdToMod.TryAdd(mod.NetId, mod))
			{
				throw new HandshakeException($"Could not register {mod.Id} as mod {mod.Id} already used netId {mod.NetId}.");
			}
		}
	}
}
