// <copyright file="ModDeclarationMessage.cs" company="miniduikboot">
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

namespace Boot.Handshake.Messages
{
	using Boot.Handshake.Enums;
	using Impostor.Api.Net.Messages;

	/// <summary>
	/// ModDeclaration message from Reactor Handshake.
	/// </summary>
	public static class ModDeclarationMessage
	{
		/// <summary>
		/// Deserialize a ModDeclaration message.
		/// </summary>
		/// <param name="reader">Reader to deserialize from.</param>
		/// <param name="netId">NetId of a mod.</param>
		/// <param name="id">String id of a mod.</param>
		/// <param name="version">Version of a mod.</param>
		/// <param name="side">Side of a mod.</param>
		public static void Deserialize(IMessageReader reader, out uint netId, out string id, out string version, out ReactorPluginSide side)
		{
			netId = reader.ReadPackedUInt32();
			id = reader.ReadString();
			version = reader.ReadString();
			side = (ReactorPluginSide)reader.ReadByte();
		}
	}
}
