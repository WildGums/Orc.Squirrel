// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using Catel.Reflection;
    using FileSystem;
    using global::Squirrel;
    using Path = Catel.IO.Path;

    /// <summary>
    /// Update service.
    /// </summary>
    public class UpdateService : IUpdateService
    {
        private const string UpdateExe = "..\\update.exe";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;
        private readonly IFileService _fileService;

        private bool _initialized;
        private string _updateExeLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService" /> class.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="fileService">The file service.</param>
        public UpdateService(IConfigurationService configurationService, IFileService fileService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => fileService);

            _configurationService = configurationService;
            _fileService = fileService;

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
                var channelName = _configurationService.GetRoamingValue(Settings.Application.AutomaticUpdates.UpdateChannel, string.Empty);

                return (from channel in AvailableChannels
                        where channel.Name.EqualsIgnoreCase(channelName)
                        select channel).FirstOrDefault();
            }
            set
            {
                Argument.IsNotNull("value", value);

                _configurationService.SetRoamingValue(Settings.Application.AutomaticUpdates.UpdateChannel, value.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check for updates.
        /// </summary>
        /// <value><c>true</c> if the check for updates is enabled; otherwise, <c>false</c>.</value>
        public bool CheckForUpdates
        {
            get { return _configurationService.GetRoamingValue(Settings.Application.AutomaticUpdates.CheckForUpdates, false); }
            set { _configurationService.SetRoamingValue(Settings.Application.AutomaticUpdates.CheckForUpdates, value); }
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
                return _fileService.Exists(updateExe);
            }
        }

        /// <summary>
        /// Gets a value indicating whether a new update has been installed.
        /// </summary>
        /// <value><c>true</c> if this instance is updated installed; otherwise, <c>false</c>.</value>
        public bool IsUpdatedInstalled { get; private set; }

        /// <summary>
        /// Occurs when a new update has begun installing.
        /// </summary>
        public event EventHandler<SquirrelEventArgs> UpdateInstalling;

        /// <summary>
        /// Occurs when a new update has been installed.
        /// </summary>
        public event EventHandler<SquirrelEventArgs> UpdateInstalled;

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

            var channels = availableChannels.ToArray();

            foreach (var channel in channels)
            {
                InitializeConfigurationKey(Settings.Application.AutomaticUpdates.GetChannelSettingName(channel.Name), channel.DefaultUrl);
            }

            AvailableChannels = channels;

            _initialized = true;
        }

        /// <summary>
        /// Checks for any available updates.
        /// </summary>
        /// <returns><c>true</c> if an update is available; otherwise <c>false</c>.</returns>
        public async Task<SquirrelResult> CheckForUpdatesAsync(SquirrelContext context)
        {
            Argument.IsNotNull(() => context);

            var result = new SquirrelResult
            {
                IsUpdateInstalledOrAvailable = false,
                CurrentVersion = GetCurrentApplicationVersion()
            };

            var channelUrl = GetChannelUrl(context);
            if (string.IsNullOrWhiteSpace(channelUrl))
            {
                return result;
            }

            using (var mgr = new UpdateManager(channelUrl))
            {
                Log.Info($"Checking for updates using url '{channelUrl}'");

                var updateInfo = await mgr.CheckForUpdate();
                if (updateInfo.ReleasesToApply.Count > 0)
                {
                    Log.Info($"Found new version '{updateInfo.FutureReleaseEntry?.Version}' using url '{channelUrl}'");

                    result.IsUpdateInstalledOrAvailable = true;
                    result.NewVersion = updateInfo.FutureReleaseEntry?.Version?.ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// Installes the available updates if there is an update available.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task<SquirrelResult> InstallAvailableUpdatesAsync(SquirrelContext context)
        {
            Argument.IsNotNull(() => context);

            var result = new SquirrelResult
            {
                IsUpdateInstalledOrAvailable = false,
                CurrentVersion = GetCurrentApplicationVersion()
            };

            var channelUrl = GetChannelUrl(context);
            if (string.IsNullOrWhiteSpace(channelUrl))
            {
                return result;
            }

            try
            {
                using (var mgr = new UpdateManager(channelUrl))
                {
                    Log.Info($"Checking for updates using url '{channelUrl}'");

                    var updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Count > 0)
                    {
                        Log.Info($"Found new version '{updateInfo.FutureReleaseEntry?.Version}' using url '{channelUrl}', installing update...");

                        result.IsUpdateInstalledOrAvailable = true;
                        result.NewVersion = updateInfo.FutureReleaseEntry?.Version?.ToString();

                        UpdateInstalling.SafeInvoke(this, () => new SquirrelEventArgs(result));

                        var releaseEntry = await mgr.UpdateApp();
                        if (releaseEntry != null)
                        {
                            Log.Info("Update installed successfully");

                            result.NewVersion = releaseEntry.Version?.ToString();
                        }
                        else
                        {
                            Log.Warning("Update finished, but no release entry was returned, falling back to previous update info");
                        }

                        IsUpdatedInstalled = true;

                        UpdateInstalled.SafeInvoke(this, () => new SquirrelEventArgs(result));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while checking for or installing the latest updates");
            }

            return result;
        }

        /// <summary>
        /// Gets the current application version.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCurrentApplicationVersion()
        {
            return AssemblyHelper.GetEntryAssembly()?.InformationalVersion();
        }

        /// <summary>
        /// Gets the channel url for the specified context.
        /// </summary>
        /// <returns>The channel url or <c>null</c> if no channel is available.</returns>
        protected string GetChannelUrl(SquirrelContext context)
        {
            if (!_initialized)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Service is not initialized, call Initialize first");
            }

            var checkForUpdates = _configurationService.GetRoamingValue<bool>(Settings.Application.AutomaticUpdates.CheckForUpdates);
            if (!checkForUpdates)
            {
                Log.Info("Automatic updates are disabled");
                return null;
            }

            var channelName = context.ChannelName ?? _configurationService.GetRoamingValue(Settings.Application.AutomaticUpdates.UpdateChannel, string.Empty);
            var channelUrlSettingsName = Settings.Application.AutomaticUpdates.GetChannelSettingName(channelName);
            var channelUrl = _configurationService.GetRoamingValue(channelUrlSettingsName, string.Empty);
            if (string.IsNullOrEmpty(channelUrl))
            {
                Log.Warning("Cannot find url for channel '{0}'", channelName);
                return null;
            }

            var entryAssemblyDirectory = AssemblyHelper.GetEntryAssembly().GetDirectory();
            var updateExe = GetUpdateExecutable();
            if (!_fileService.Exists(updateExe))
            {
                Log.Warning("Cannot check for updates, update.exe is not available");
                return null;
            }

            return channelUrl;
        }

        private string GetUpdateExecutable()
        {
            if (string.IsNullOrWhiteSpace(_updateExeLocation))
            {
                var entryAssemblyDirectory = AssemblyHelper.GetEntryAssembly().GetDirectory();
                _updateExeLocation = Path.GetFullPath(UpdateExe, entryAssemblyDirectory);

                Log.Debug($"Determined update executable path '{_updateExeLocation}', exists: {_fileService.Exists(_updateExeLocation)}");
            }

            return _updateExeLocation;
        }

        private void InitializeConfigurationKey(string key, object defaultValue)
        {
            _configurationService.InitializeRoamingValue(key, defaultValue);
        }
    }
}