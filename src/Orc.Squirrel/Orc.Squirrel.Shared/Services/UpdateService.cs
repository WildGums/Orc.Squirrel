// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateService.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using Catel.Reflection;
    using Orc.Squirrel;
    using Path = Catel.IO.Path;

    /// <summary>
    /// Update service.
    /// </summary>
    public class UpdateService : IUpdateService
    {
        private const string UpdateExe = "..\\update.exe";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;

        private bool _initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService"/> class.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        public UpdateService(IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _configurationService = configurationService;

            AvailableChannels = new UpdateChannel[] { };
        }

        /// <summary>
        /// Gets the available availableChannels.
        /// </summary>
        /// <value>The availableChannels.</value>
        public UpdateChannel[] AvailableChannels { get; private set; }

        /// <summary>
        /// Gets or sets the current channel.
        /// </summary>
        /// <value>The current channel.</value>
        public UpdateChannel CurrentChannel
        {
            get
            {
                var channelName = _configurationService.GetValue(Settings.Application.AutomaticUpdates.UpdateChannel, string.Empty);
                return (from channel in AvailableChannels
                        where string.Equals(channel.Name, channelName)
                        select channel).FirstOrDefault();
            }
            set
            {
                Argument.IsNotNull("value", value);

                _configurationService.SetValue(Settings.Application.AutomaticUpdates.UpdateChannel, value.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check for updates.
        /// </summary>
        /// <value><c>true</c> if the check for updates is enabled; otherwise, <c>false</c>.</value>
        public bool CheckForUpdates
        {
            get { return _configurationService.GetValue(Settings.Application.AutomaticUpdates.CheckForUpdates, false); }
            set { _configurationService.SetValue(Settings.Application.AutomaticUpdates.CheckForUpdates, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the update system is available.
        /// </summary>
        /// <value><c>true</c> if the is update system is available; otherwise, <c>false</c>.</value>
        public bool IsUpdateSystemAvailable
        {
            get
            {
                var updateExe = GetUpdateExecutable();
                return File.Exists(updateExe);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a new update has been installed.
        /// </summary>
        /// <value><c>true</c> if this instance is updated installed; otherwise, <c>false</c>.</value>
        public bool IsUpdatedInstalled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether an update outside the maintenance is available.
        /// </summary>
        /// <value><c>true</c> if an update outside the maintenance is available; otherwise, <c>false</c>.</value>
        public bool IsUpdateOutsideMaintenanceAvailable { get; private set; }

        /// <summary>
        /// Occurs when a new update has been installed.
        /// </summary>
        public event EventHandler<EventArgs> UpdateInstalled;

        /// <summary>
        /// Occurs when an update is available but not installed because it is outside the maintenance window (specified by maximum release date).
        /// </summary>
        public event EventHandler<EventArgs> UpdateOutsideMaintenanceAvailable;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="availableChannels">The available channels.</param>
        /// <param name="defaultChannel">The default channel.</param>
        /// <param name="defaultCheckForUpdatesValue">The default value for the check for updates setting.</param>
        public void Initialize(IEnumerable<UpdateChannel> availableChannels, UpdateChannel defaultChannel, bool defaultCheckForUpdatesValue)
        {
            InitializeConfigurationKey(Settings.Application.AutomaticUpdates.CheckForUpdates, defaultCheckForUpdatesValue);
            InitializeConfigurationKey(Settings.Application.AutomaticUpdates.UpdateChannel, defaultChannel.Name);

            foreach (var channel in availableChannels)
            {
                InitializeConfigurationKey(Settings.Application.AutomaticUpdates.GetChannelSettingName(channel.Name), channel.DefaultUrl);
            }

            AvailableChannels = availableChannels.ToArray();

            _initialized = true;
        }

        /// <summary>
        /// Handles the updates by installing them if there is an update available.
        /// </summary>
        /// <param name="maximumReleaseDate">The maximum release date.</param>
        /// <returns>Task.</returns>
        public async Task HandleUpdates(DateTime? maximumReleaseDate = null)
        {
            if (!_initialized)
            {
                Log.ErrorAndThrowException<InvalidOperationException>("Service is not initialized, call Initialize first");
                return;
            }

            var checkForUpdates = _configurationService.GetValue<bool>(Settings.Application.AutomaticUpdates.CheckForUpdates);
            if (!checkForUpdates)
            {
                Log.Info("Automatic updates are disabled");
                return;
            }

            var channelName = _configurationService.GetValue<string>(Settings.Application.AutomaticUpdates.UpdateChannel, string.Empty);
            var channelUrlSettingsName = Settings.Application.AutomaticUpdates.GetChannelSettingName(channelName);
            var channelUrl = _configurationService.GetValue<string>(channelUrlSettingsName, string.Empty);
            if (string.IsNullOrEmpty(channelUrl))
            {
                Log.Warning("Cannot find url for channel '{0}'", channelName);
                return;
            }

            var entryAssemblyDirectory = AssemblyHelper.GetEntryAssembly().GetDirectory();
            var updateExe = GetUpdateExecutable();
            if (!File.Exists(updateExe))
            {
                Log.Warning("Cannot check for updates, update.exe is not available");
                return;
            }

            Log.Info("Calling update.exe for url '{0}'", channelUrl);

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    if (!maximumReleaseDate.HasValue)
                    {
                        maximumReleaseDate = DateTime.MaxValue;
                    }

                    var startInfo = new ProcessStartInfo(updateExe);
                    startInfo.Arguments = string.Format("--update={0} --md={1} --silent", channelUrl, maximumReleaseDate.Value.ToString("yyyyMMddHHmmss"));
                    startInfo.WorkingDirectory = Path.GetFullPath("..", entryAssemblyDirectory);
                    startInfo.UseShellExecute = true;
                    startInfo.CreateNoWindow = true;

                    var process = Process.Start(startInfo);
                    process.WaitForExit();

                    Log.Debug("Update.exe exited with exit code '{0}'", process.ExitCode);

                    // Possible exit codes:
                    // -1 => An error occurred. Check the log file for more information about this error
                    //  0 => No errors, no additional information available
                    //  1 => New version available or new version is installed successfully (depending on switch /checkonly)
                    //  2 => New version which is mandatory (forced) is available (for the future?)
                    //  3 => No new version available

                    switch (process.ExitCode)
                    {
                        case 1:
                            IsUpdatedInstalled = true;
                            UpdateInstalled.SafeInvoke(this);

                            Log.Info("Installed new update");
                            break;

                        case 4:
                            IsUpdateOutsideMaintenanceAvailable = true;
                            UpdateOutsideMaintenanceAvailable.SafeInvoke(this);

                            Log.Info("New update is available but is outside maintenance, maintenance ended on '{0}'", maximumReleaseDate);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to check for updates");
                }
            });
        }

        private string GetUpdateExecutable()
        {
            var entryAssemblyDirectory = AssemblyHelper.GetEntryAssembly().GetDirectory();
            var updateExe = Path.GetFullPath(UpdateExe, entryAssemblyDirectory);

            return updateExe;
        }

        private void InitializeConfigurationKey(string key, object defaultValue)
        {
            _configurationService.InitializeValue(key, defaultValue);
        }
    }
}