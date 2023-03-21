namespace Orc.Squirrel;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Update service.
/// </summary>
public interface IUpdateService
{
    /// <summary>
    /// Gets the available availableChannels.
    /// </summary>
    /// <value>The availableChannels.</value>
    UpdateChannel[] AvailableChannels { get; }

    /// <summary>
    /// Gets a value indicating whether the update system is available.
    /// </summary>
    /// <value><c>true</c> if the is update system is available; otherwise, <c>false</c>.</value>
    bool IsUpdateSystemAvailable { get; }

    /// <summary>
    /// Gets a value indicating whether a new update has been installed.
    /// </summary>
    /// <value><c>true</c> if this instance is updated installed; otherwise, <c>false</c>.</value>
    bool IsUpdatedInstalled { get; }

    /// <summary>
    /// Occurs when a new update has begun installing.
    /// </summary>
    event EventHandler<SquirrelEventArgs>? UpdateInstalling;

    /// <summary>
    /// Occurs when a new update has been installed.
    /// </summary>
    event EventHandler<SquirrelEventArgs>? UpdateInstalled;

    /// <summary>
    /// Occurs when the update progress changes.
    /// </summary>
    event EventHandler<SquirrelProgressEventArgs>? UpdateProgress;

    /// <summary>
    /// Current channel
    /// </summary>
    UpdateChannel? CurrentChannel { get; set; }

    /// <summary>
    /// Current state for updates check
    /// </summary>
    bool IsCheckForUpdatesEnabled { get; set; }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="availableChannels">The available channels.</param>
    /// <param name="defaultChannel">The default channel.</param>
    /// <param name="defaultCheckForUpdatesValue">The default value for the check for updates setting.</param>
    Task InitializeAsync(IEnumerable<UpdateChannel> availableChannels, UpdateChannel defaultChannel, bool defaultCheckForUpdatesValue);

    /// <summary>
    /// Checks for any available updates.
    /// </summary>
    /// <returns><c>true</c> if an update is available; otherwise <c>false</c>.</returns>
    Task<SquirrelResult> CheckForUpdatesAsync(SquirrelContext context);

    /// <summary>
    /// Installes the available updates if there is an update available.
    /// </summary>
    /// <returns>Task.</returns>
    Task<SquirrelResult> InstallAvailableUpdatesAsync(SquirrelContext context);
}
