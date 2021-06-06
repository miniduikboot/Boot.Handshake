// <copyright file="ModListManagerCleaner.cs" company="miniduikboot">
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
	using System.Threading;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;

	/// <summary>
	/// Periodically remove disconnected clients from the mod list manager.
	/// </summary>
	public sealed class ModListManagerCleaner : IHostedService, IDisposable
	{
		private readonly ILogger<ModListManagerCleaner> logger;
		private readonly ModListManager listManager;
		private Timer? timer;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModListManagerCleaner"/> class.
		/// </summary>
		/// <param name="logger">Log periodic activity.</param>
		/// <param name="listManager">The list manager to clean.</param>
		public ModListManagerCleaner(ILogger<ModListManagerCleaner> logger, ModListManager listManager)
		{
			this.logger = logger;
			this.listManager = listManager;
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			this.timer?.Dispose();
		}

		/// <inheritdoc/>
		public Task StartAsync(CancellationToken cancellationToken)
		{
			this.logger.LogInformation("Boot.Handshake starting");
			this.timer = new Timer(this.DoCleanup, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		public Task StopAsync(CancellationToken cancellationToken)
		{
			this.logger.LogInformation("Boot.Handshake stopping");
			_ = this.timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		private void DoCleanup(object? state)
		{
			this.listManager.Cleanup();
		}
	}
}
