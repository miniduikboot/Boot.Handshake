// <copyright file="ModdedHandshakeS2C.cs" company="miniduikboot">
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

	/**
	 * <summary>
	 * Serialize response to first part of handshake.
	 * </summary>
	  */
	public static class ModdedHandshakeS2C
	{
		/**
		 * <summary>
		 * Deserialize initial handshake containing the amount of mods.
		 * </summary>
		 * <param name="writer">The message reader containing the rpc.</param>
		 * <param name="serverName">Name of the server in use.</param>
		 * <param name="serverVersion">Version of the server in use.</param>
		 * <param name="pluginCount">The amount of plugins that will be declared.</param>
		 */
		public static void Serialize(IMessageWriter writer, string serverName, string serverVersion, int pluginCount)
		{
			writer.Write((byte)ReactorMessageTypes.Handshake);
			writer.Write(serverName);
			writer.Write(serverVersion);
			writer.WritePacked(pluginCount);
		}
	}
}
