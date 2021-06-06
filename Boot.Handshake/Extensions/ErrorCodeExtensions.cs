// <copyright file="ErrorCodeExtensions.cs" company="miniduikboot">
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

namespace Boot.Handshake.Extensions
{
	using System;
	using Boot.Handshake.Enums;
	using Impostor.Api.Games;

	/// <summary>
	/// Provides extensions for the ErrorCode enum.
	/// </summary>
	public static class ErrorCodeExtensions
	{
		/// <summary>
		/// Gets a colored message that is to be sent to the client.
		/// </summary>
		/// <param name="code">The error code.</param>
		/// <param name="parameters">Parameters for formatting.</param>
		/// <returns>The formatted error message.</returns>
		public static string GetClientMessage(this ErrorCode code, params object?[] parameters)
		{
			return code switch
			{
				ErrorCode.BHS01 => string.Format("<color=\"red\">Error BHS01:</color> Mod {0} was already registered.", parameters),

				ErrorCode.BHS02 => string.Format(
					"<color=\"red\">Error BHS02:</color> Could not register {0} as mod {1} already used netId {2}.",
					parameters),

				ErrorCode.BHS03 => throw new NotImplementedException("BHS03 is not implemented."),

				ErrorCode.BHS10 => "<color=\"red\">Error BHS10:</color> Sorry, you cannot join an non-modded lobby with mods.",

				ErrorCode.BHS11 =>"<color=\"red\">Error BHS11:</color> Sorry, you cannot join a modded lobby without mods.",

				ErrorCode.BHS20 => string.Format("<color=\"red\">Error BHS20:</color> Host does not have a mod called {0}", parameters),

				ErrorCode.BHS21 => string.Format("<color=\"red\">Error BHS21:</color> You are missing a mod called {0}", parameters),

				ErrorCode.BSH22 => string.Format(
					"<color=\"red\">Error BHS22:</color> Version of {0} does not match: you have {1}, host has {2}.",
					parameters),

				_ => throw new ArgumentOutOfRangeException(nameof(code), code, "The specified error code is invalid.")
			};
		}

		/// <summary>
		/// Gets a GameJoinResult error from the specified error code.
		/// </summary>
		/// <param name="code">The error code.</param>
		/// <param name="parameters">Parameters for formatting.</param>
		/// <returns>The GameJoinResult to be sent to the client.</returns>
		public static GameJoinResult GetJoinResult(this ErrorCode code, params object?[] parameters)
		{
			return GameJoinResult.CreateCustomError(code.GetClientMessage(parameters));
		}
	}
}
