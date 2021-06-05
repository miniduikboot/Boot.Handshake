// <copyright file="ClientMod.cs" company="miniduikboot">
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
	using Boot.Handshake.Enums;

	/// <summary>
	/// Server-side representation of a client mod that was declared via the handshake.
	/// </summary>
	public class ClientMod
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ClientMod"/> class.
		/// </summary>
		/// <param name="netId">NetId of the mod as declared by the client.</param>
		/// <param name="id">Id of the mod.</param>
		/// <param name="version">Version of the mod.</param>
		/// <param name="side">The side the mod is on.</param>
		public ClientMod(uint netId, string id, string version, ReactorPluginSide side)
		{
			this.NetId = netId;
			this.Id = id;
			this.Version = version;
			this.Side = side;
		}

		/// <summary>
		/// Gets the NetId of this mod.
		/// </summary>
		public uint NetId { get; }

		/// <summary>
		/// Gets the Id of this mod, which should be unique and follow reverse domain notation.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the Version of this mod, which can be any string.
		/// </summary>
		public string Version { get; }

		/// <summary>
		/// Gets the Plugin Side of this mod.
		/// </summary>
		public ReactorPluginSide Side { get; }

		/// <inheritdoc/>
		public override bool Equals(object? obj)
		{
			return obj is ClientMod mod &&
				   this.NetId == mod.NetId &&
				   this.Id == mod.Id &&
				   this.Version == mod.Version &&
				   this.Side == mod.Side;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return HashCode.Combine(this.NetId, this.Id, this.Version, this.Side);
		}
	}
}
