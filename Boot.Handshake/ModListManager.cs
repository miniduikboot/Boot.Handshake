// <copyright file="ModListManager.cs" company="miniduikboot">
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
	using System.Collections.Concurrent;
	using System.Diagnostics.CodeAnalysis;
	using Impostor.Api.Net;

	/// <summary>
	/// Contains a list of ModList for each connected client.
	/// </summary>
	public class ModListManager
	{
		[SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1000:KeywordsMustBeSpacedCorrectly", Justification = "Stylecop documentation does not match implementation")]
		private readonly ConcurrentDictionary<IClient, ModList> lists = new();

		/// <summary>
		/// Add a mod list.
		/// </summary>
		/// <param name="client">The client to add a mod list to.</param>
		/// <param name="list">The mod list that will be added.</param>
		/// <returns>If the mod list was register or if another list was already registered.</returns>
		public bool Add(IClient client, ModList list)
		{
			return this.lists.TryAdd(client, list);
		}

		/// <summary>
		/// Get the ModList for a given client.
		/// </summary>
		/// <param name="client">The client to get the modlist of.</param>
		/// <returns>The modlist of a client or null if there is no list.</returns>
		public ModList? Get(IClient client)
		{
			return this.lists.TryGetValue(client, out var result) ? result : null;
		}

		/// <summary>
		/// Check if a certain player has a mod list.
		/// </summary>
		/// <param name="client">The client you want to get a mod list from.</param>
		/// <returns>If the provided client has an mod list.</returns>
		public bool ContainsList(IClient client)
		{
			return this.lists.ContainsKey(client);
		}

		/// <summary>
		/// Remove the ModList of a <paramref name="client"/>.
		/// </summary>
		/// <param name="client">The client whose ModList should be removed.</param>
		public void Destroy(IClient client)
		{
			_ = this.lists.TryRemove(client, out _);
		}
	}
}
