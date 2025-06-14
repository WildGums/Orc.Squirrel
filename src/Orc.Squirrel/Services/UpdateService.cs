﻿namespace Orc.Squirrel;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Catel;
using Catel.Configuration;
using Catel.Logging;
using Catel.Reflection;
using FileSystem;
using global::Velopack;
using global::Velopack.Locators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;
using Velopack;
using Path = Catel.IO.Path;

/// <summary>
/// Update service.
/// </summary>
public class UpdateService : IUpdateService
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IConfigurationService _configurationService;
    private readonly IFileService _fileService;
    private readonly IUpdateExecutableLocationService _updateExecutableLocationService;
    private readonly IAppMetadataProvider _appMetadataProvider;
    private readonly IVelopackLocator _velopackLocator;

    private bool _initialized;

    public UpdateService(IConfigurationService configurationService, IFileService fileService,
        IUpdateExecutableLocationService updateExecutableLocationService,
        IAppMetadataProvider appMetadataProvider, IVelopackLocator velopackLocator)
    {
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(fileService);
        ArgumentNullException.ThrowIfNull(updateExecutableLocationService);
        ArgumentNullException.ThrowIfNull(appMetadataProvider);

        _configurationService = configurationService;
        _fileService = fileService;
        _updateExecutableLocationService = updateExecutableLocationService;
        _appMetadataProvider = appMetadataProvider;
        _velopackLocator = velopackLocator;

        AvailableChannels = Array.Empty<UpdateChannel>();
    }

    /// <summary>
    /// Gets the available availableChannels.
    /// </summary>
    /// <value>The availableChannels.</value>
    public UpdateChannel[] AvailableChannels { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the update system is available.
    /// </summary>
    /// <value><c>true</c> if the is update system is available; otherwise, <c>false</c>.</value>
    public bool IsUpdateSystemAvailable
    {
        get
        {
            var updateExe = _updateExecutableLocationService.FindUpdateExecutable();
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
    public event EventHandler<SquirrelEventArgs>? UpdateInstalling;

    /// <summary>
    /// Occurs when a progress update happens.
    /// </summary>
    public event EventHandler<SquirrelProgressEventArgs>? UpdateProgress;

    /// <summary>
    /// Occurs when a new update has been installed.
    /// </summary>
    public event EventHandler<SquirrelEventArgs>? UpdateInstalled;

    public UpdateChannel? CurrentChannel
    {
        get => GetCurrentChannel();
        set => SetCurrentChannel(value);
    }

    protected virtual UpdateChannel? GetCurrentChannel()
    {
        var channelName = _configurationService.GetRoamingValue(Settings.Application.AutomaticUpdates.UpdateChannel, string.Empty);

        return (from channel in AvailableChannels
                where channel.Name.EqualsIgnoreCase(channelName)
                select channel).FirstOrDefault();
    }

    protected virtual void SetCurrentChannel(UpdateChannel? updateChannel)
    {
        ArgumentNullException.ThrowIfNull(updateChannel);

        _configurationService.SetRoamingValue(Settings.Application.AutomaticUpdates.UpdateChannel, updateChannel.Name);
    }

    public bool IsCheckForUpdatesEnabled
    {
        get => GetCheckForUpdatesValue();
        set => SetCheckForUpdatesValue(value);
    }

    protected virtual bool GetCheckForUpdatesValue()
    {
        return _configurationService.GetRoamingValue(Settings.Application.AutomaticUpdates.CheckForUpdates, false);
    }

    protected virtual void SetCheckForUpdatesValue(bool value)
    {
        _configurationService.SetRoamingValue(Settings.Application.AutomaticUpdates.CheckForUpdates, value);
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="availableChannels">The available channels.</param>
    /// <param name="defaultChannel">The default channel.</param>
    /// <param name="defaultCheckForUpdatesValue">The default value for the check for updates setting.</param>
    public virtual async Task InitializeAsync(IEnumerable<UpdateChannel> availableChannels, UpdateChannel defaultChannel, bool defaultCheckForUpdatesValue)
    {
        ArgumentNullException.ThrowIfNull(availableChannels);
        ArgumentNullException.ThrowIfNull(defaultChannel);

        await InitializeConfigurationKeyAsync(Settings.Application.AutomaticUpdates.CheckForUpdates, defaultCheckForUpdatesValue);
        await InitializeConfigurationKeyAsync(Settings.Application.AutomaticUpdates.UpdateChannel, defaultChannel.Name);

        var channels = availableChannels.ToArray();

        foreach (var channel in channels)
        {
            await InitializeConfigurationKeyAsync(Settings.Application.AutomaticUpdates.GetChannelSettingName(channel.Name), channel.DefaultUrl);
        }

        AvailableChannels = channels;

        _initialized = true;
    }

    /// <summary>
    /// Checks for any available updates.
    /// </summary>
    /// <returns><c>true</c> if an update is available; otherwise <c>false</c>.</returns>
    public virtual async Task<SquirrelResult> CheckForUpdatesAsync(SquirrelContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var result = new SquirrelResult
        {
            IsUpdateInstalledOrAvailable = false,
            CurrentVersion = _appMetadataProvider.CurrentVersion ?? AssemblyHelper.GetRequiredEntryAssembly().InformationalVersion() ?? string.Empty
        };

        var channelUrl = GetChannelUrl(context);
        if (string.IsNullOrWhiteSpace(channelUrl))
        {
            return result;
        }

        Log.Info($"Checking for updates, current version is '{result.CurrentVersion}'");

        // Step 1: check using Velopack
        try
        {
            var locator = _velopackLocator;
            if (locator is SquirrelVelopackLocator squirrelVelopackLocator)
            {
                squirrelVelopackLocator.UpdateAppId(_appMetadataProvider.AppId);

                if (_appMetadataProvider.CurrentVersion is not null)
                {
                    squirrelVelopackLocator.UpdateCurrentlyInstalledVersion(SemanticVersion.Parse(_appMetadataProvider.CurrentVersion));
                }
            }

            var velopackUpdateManager = new UpdateManager(channelUrl, locator: locator);

            var newVersion = await velopackUpdateManager.CheckForUpdatesAsync();
            if (newVersion is not null &&
                newVersion.TargetFullRelease.Version.ToFullString() != result.CurrentVersion)
            {
                result.IsUpdateInstalledOrAvailable = true;
                result.NewVersion = newVersion.TargetFullRelease.Version.ToString();

                if (!result.IsUpdateInstalledOrAvailable)
                {
                    Log.Info("No updates available");
                }
                else
                {
                    Log.Info($"Found new version '{result.NewVersion}' using url '{channelUrl}'");
                }

                return result;
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            if (httpRequestException.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                Log.Error(httpRequestException, "An error occurred while checking for the latest updates using Velopack");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while checking for the latest updates using Velopack");
        }

        // Step 2: check using Squirrel (only if update.exe exists)
        var updateExe = _updateExecutableLocationService.FindUpdateExecutable();
        if (_fileService.Exists(updateExe))
        {
            try
            {
                var startInfo = CreateUpdateProcessStartInfo($"--checkForUpdate={channelUrl}");
                using var process = Process.Start(startInfo);
                if (process is null)
                {
                    return result;
                }

                var output = await process.StandardOutput.ReadToEndAsync();

#pragma warning disable CL0001
                process.WaitForExit();
#pragma warning restore CL0001

                var startIndex = output.IndexOf("{");
                if (startIndex > 0)
                {
                    output = output[startIndex..];
                }

                // Results similar to this:
                //{
                //    "currentVersion": "2.3.0-alpha1013",
                //    "futureVersion": "2.3.0-alpha1094",
                //    "releasesToApply": [
                //        {
                //            "version": "2.3.0-alpha1039",
                //            "releaseNotes": ""
                //        },
                //        {
                //            "version": "2.3.0-alpha1074",
                //            "releaseNotes": ""
                //        },
                //        {
                //            "version": "2.3.0-alpha1094",
                //            "releaseNotes": ""
                //        }
                //    ]
                //}

                if (!string.IsNullOrWhiteSpace(output))
                {
                    dynamic releaseInfo = JObject.Parse(output);

                    foreach (var releaseToApply in releaseInfo.releasesToApply)
                    {
                        result.IsUpdateInstalledOrAvailable = true;
                        result.NewVersion = releaseToApply.version;
                    }
                }

                if (!result.IsUpdateInstalledOrAvailable)
                {
                    Log.Info("No updates available");
                }
                else
                {
                    Log.Info($"Found new version '{result.NewVersion}' using url '{channelUrl}'");
                }
            }
            catch (JsonReaderException)
            {
                // Expected when migrating to Velopack, ignore it
            }
            catch (HttpRequestException httpRequestException)
            {
                if (httpRequestException.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Log.Error(httpRequestException, "An error occurred while checking for the latest updates using Squirrel");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while checking for the latest updates using Squirrel");
            }
        }

        return result;
    }

    /// <summary>
    /// Installs the available updates if there is an update available.
    /// </summary>
    /// <returns>Task.</returns>
    public virtual async Task<SquirrelResult> InstallAvailableUpdatesAsync(SquirrelContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var result = new SquirrelResult
        {
            IsUpdateInstalledOrAvailable = false,
            CurrentVersion = _appMetadataProvider.CurrentVersion ?? AssemblyHelper.GetRequiredEntryAssembly().InformationalVersion() ?? string.Empty
        };

        var channelUrl = GetChannelUrl(context);
        if (string.IsNullOrWhiteSpace(channelUrl))
        {
            return result;
        }

        // Step 1: Velopack
        try
        {
            var locator = _velopackLocator;
            if (locator is SquirrelVelopackLocator squirrelVelopackLocator)
            {
                squirrelVelopackLocator.UpdateAppId(_appMetadataProvider.AppId);

                if (_appMetadataProvider.CurrentVersion is not null)
                {
                    squirrelVelopackLocator.UpdateCurrentlyInstalledVersion(SemanticVersion.Parse(_appMetadataProvider.CurrentVersion));
                }
            }

            var velopackUpdateManager = new UpdateManager(channelUrl, locator: locator);

            var newVersion = await velopackUpdateManager.CheckForUpdatesAsync();
            if (newVersion is not null &&
                newVersion.TargetFullRelease.Version.ToFullString() != result.CurrentVersion)
            {
                Log.Info($"Installing (downloading) {newVersion.TargetFullRelease.Version} using base release {newVersion.BaseRelease?.Version}, current version is {result.CurrentVersion}");

                result.NewVersion = newVersion.TargetFullRelease.Version.ToString();

                UpdateInstalling?.Invoke(this, new SquirrelEventArgs(result));

                await velopackUpdateManager.DownloadUpdatesAsync(newVersion, x =>
                {
                    RaiseProgressChanged(x);
                });

                result.IsUpdateInstalledOrAvailable = true;

                Log.Info("Update installed (downloaded) successfully");

                IsUpdatedInstalled = true;

                UpdateInstalled?.Invoke(this, new SquirrelEventArgs(result));

                // No need to restart / apply, according to the docs, Velopack will automatically
                // apply the updates when the application is restarted

                return result;
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            if (httpRequestException.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                Log.Error(httpRequestException, "An error occurred while checking for or installing the latest updates using Velopack");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while checking for or installing the latest updates using Velopack");
        }

        // Step 2: Squirrel
        try
        {
            // Do we actually have an update? Do a quick one here
            var checkResult = await CheckForUpdatesAsync(context);

            // Note that we don't want the process to stop updating, we only want to invoke
            if (checkResult.IsUpdateInstalledOrAvailable)
            {
                Log.Info($"Found new version '{checkResult.NewVersion}' using url '{channelUrl}', installing update...");

                result.NewVersion = checkResult.NewVersion;

                UpdateInstalling?.Invoke(this, new SquirrelEventArgs(result));
            }
            else
            {
                Log.Info($"Could not determine whether a new version was available for certain, going to run update anyway...");
            }

            // Executable wrapper
            var startInfo = CreateUpdateProcessStartInfo($"--update={channelUrl}");
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return result;
            }

            var line = "0";

            while (!string.IsNullOrWhiteSpace(line))
            {
                if (int.TryParse(line, out var progress))
                {
                    RaiseProgressChanged(progress);
                }

                line = await process.StandardOutput.ReadLineAsync();
            }

#pragma warning disable CL0001
            process.WaitForExit();
#pragma warning restore CL0001

            // Only when we knew there was an update pending, we notify
            if (process.ExitCode == 0 && checkResult.IsUpdateInstalledOrAvailable)
            {
                result.NewVersion = checkResult.NewVersion ?? "unknown";
                result.IsUpdateInstalledOrAvailable = true;

                Log.Info("Update installed successfully");

                IsUpdatedInstalled = true;

                UpdateInstalled?.Invoke(this, new SquirrelEventArgs(result));

                return result;
            }
        }
        catch (HttpRequestException httpRequestException)
        {
            if (httpRequestException.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                Log.Error(httpRequestException, "An error occurred while checking for or installing the latest updates using Squirrel");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while checking for or installing the latest updates using Squirrel");
        }

        return result;
    }

    private ProcessStartInfo CreateUpdateProcessStartInfo(string arguments)
    {
        var updateExecutable = _updateExecutableLocationService.FindUpdateExecutable();
        //var executableFileName = _updateExecutableLocationService.GetApplicationExecutable();

        var startInfo = new ProcessStartInfo(updateExecutable)
        {
            Arguments = arguments,
            WorkingDirectory = Path.GetDirectoryName(updateExecutable),
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return startInfo;
    }

    protected virtual void RaiseProgressChanged(int percentage)
    {
        Log.Debug($"Update progress: {percentage}%");

        UpdateProgress?.Invoke(this, new SquirrelProgressEventArgs(percentage));
    }

    /// <summary>
    /// Gets the channel url for the specified context.
    /// </summary>
    /// <returns>The channel url or <c>null</c> if no channel is available.</returns>
    protected virtual string? GetChannelUrl(SquirrelContext context)
    {
        if (!_initialized)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>("Service is not initialized, call Initialize first");
        }

        var checkForUpdates = GetCheckForUpdatesValue();
        if (!checkForUpdates)
        {
            Log.Info("Automatic updates are disabled");
            return null;
        }

        var channelName = context.ChannelName;
        if (string.IsNullOrWhiteSpace(channelName))
        {
            channelName = _configurationService.GetRoamingValue(Settings.Application.AutomaticUpdates.UpdateChannel, string.Empty);
        }

        var channelUrlSettingsName = Settings.Application.AutomaticUpdates.GetChannelSettingName(channelName);
        var channelUrl = _configurationService.GetRoamingValue(channelUrlSettingsName, string.Empty);
        if (string.IsNullOrEmpty(channelUrl))
        {
            Log.Warning("Cannot find url for channel '{0}'", channelName);
            return null;
        }

        return channelUrl;
    }

    private async Task InitializeConfigurationKeyAsync(string key, object defaultValue)
    {
        _configurationService.InitializeRoamingValue(key, defaultValue);
    }
}
