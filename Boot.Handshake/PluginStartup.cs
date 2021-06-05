// <copyright file="PluginStartup.cs" company="miniduikboot">
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
	using Boot.Handshake.Handlers;
	using Impostor.Api.Events;
	using Impostor.Api.Plugins;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;

	/**
	 * <summary>
	 * Register all EventListeners.
	 * </summary>
	  */
	internal class PluginStartup : IPluginStartup
	{
		/// <inheritdoc/>
		public void ConfigureHost(IHostBuilder host)
		{
			// Method intentionally left empty.
		}

		/// <inheritdoc/>
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IEventListener, ClientEventListener>();
			services.AddSingleton<IEventListener, GameEventListener>();
			services.AddSingleton<ModListManager>();
			services.AddSingleton<ReactorRootMessage>();
		}
	}
}
