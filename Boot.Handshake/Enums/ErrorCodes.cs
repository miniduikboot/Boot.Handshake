// <copyright file="ErrorCodes.cs" company="miniduikboot">
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
	/// Error codes related to the mod lists and versions.
	/// </summary>
	public enum ErrorCode
	{
		/// <summary>
		/// Mod registered twice
		/// </summary>
		BHS01,

		/// <summary>
		/// Other mod already uses NetId
		/// </summary>
		BHS02,

		/// <summary>
		/// Cannot accept mod declaration when initial handshake was not made
		/// </summary>
		BHS03,

		/// <summary>
		/// Cannot join non-modded lobby with mods
		/// </summary>
		BHS10,

		/// <summary>
		/// Cannot join modded lobby without mods
		/// </summary>
		BHS11,

		/// <summary>
		/// Host doesn't have mod X
		/// </summary>
		BHS20,

		/// <summary>
		/// Client doesn't have mod X
		/// </summary>
		BHS21,

		/// <summary>
		/// Version mismatch
		/// </summary>
		BSH22,
	}
}
