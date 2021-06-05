// <copyright file="HandshakePlugin.cs" company="miniduikboot">
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
	using System.Threading.Tasks;
	using Impostor.Api.Net.Custom;
	using Impostor.Api.Plugins;

	/// <summary>
	/// This plugin deals with the Reactor Handshake.
	/// </summary>
	[ImpostorPlugin("at.duikbo.handshake")]
	internal class HandshakePlugin : PluginBase
	{
		private readonly ICustomMessageManager<ICustomRootMessage> rootMessageManager;
		private readonly ReactorRootMessage reactorRootMessage;

		/// <summary>
		/// Initializes a new instance of the <see cref="HandshakePlugin"/> class.
		/// </summary>
		/// <param name="rootMessageManager">Manager of the Root Messages.</param>
		/// <param name="reactorRootMessage">The root message we want to register.</param>
		public HandshakePlugin(ICustomMessageManager<ICustomRootMessage> rootMessageManager, ReactorRootMessage reactorRootMessage)
		{
			this.rootMessageManager = rootMessageManager;
			this.reactorRootMessage = reactorRootMessage;
		}

		/// <inheritdoc/>
		public override ValueTask EnableAsync()
		{
			_ = this.rootMessageManager.Register(this.reactorRootMessage);
			return default;
		}
	}
}
