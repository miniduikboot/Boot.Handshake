// <copyright file="ReactorPluginSide.cs" company="miniduikboot">
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

namespace Boot.Handshake.Enums
{
	/// <summary>
	/// PLugin side attribute from Reactor source code.
	/// </summary>
	public enum ReactorPluginSide : byte
	{
		/// <summary>
		/// Required by both sides, reject connection if missing on the other side
		/// </summary>
		Both,

		/// <summary>
		/// Required only by client
		/// </summary>
		ClientOnly,
	}
}
