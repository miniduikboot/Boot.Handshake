// <copyright file="ModdedHandshakeC2S.cs" company="miniduikboot">
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
	using Impostor.Api.Net.Messages;

	/**
	 * <summary>
	 * Deserialize first part of the handshake.
	 * </summary>
	  */
	public static class ModdedHandshakeC2S
	{
		/**
		 * <summary>
		 * Deserialize initial handshake containing the amount of mods.
		 * </summary>
		 * <param name="reader">The message reader containing the handshake.</param>
		 * <param name="protocolVersion">Version of the protocol in use.</param>
		 * <param name="modCount">The amount of mods that will be declared.</param>
		 */
		public static void Deserialize(IMessageReader reader, out byte protocolVersion, out int modCount)
		{
			protocolVersion = reader.ReadByte();
			modCount = reader.ReadPackedInt32();
		}
	}
}
