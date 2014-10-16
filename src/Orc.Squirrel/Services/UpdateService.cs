// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2014 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orcomp.Squirrel
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
        /// <returns>Task.</returns>
        public async Task HandleUpdates()
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
            var updateExe = Path.GetFullPath(UpdateExe, entryAssemblyDirectory);
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
                    var startInfo = new ProcessStartInfo(updateExe);
                    startInfo.Arguments = string.Format("--update={0} --silent", channelUrl);
                    startInfo.WorkingDirectory = Path.GetFullPath("..", entryAssemblyDirectory);

                    var process = Process.Start(startInfo);
                    process.WaitForExit();

                    Log.Info("Update.exe exited with exit code '{0}'", process.ExitCode);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to check for updates, an error occurred");
                }
            });
        }

        private void InitializeConfigurationKey(string key, object defaultValue)
        {
            _configurationService.InitializeValue(key, defaultValue);
        }
    }
}