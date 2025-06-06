﻿namespace Orc.Squirrel.Velopack
{
    using System.Diagnostics;
    using global::Velopack.Locators;
    using global::Velopack.Logging;
    using NuGet.Versioning;

    internal class SquirrelVelopackLocator : WindowsVelopackLocator
    {
        private string? _appId;
        private SemanticVersion? _currentlyInstalledVersion;
        private string? _packagesDir;
        private string? _updateExePath;

        public SquirrelVelopackLocator()
            : this(Process.GetCurrentProcess().MainModule?.FileName!, (uint)Process.GetCurrentProcess().Id, null)
        {
            
        }

        public SquirrelVelopackLocator(string currentProcessPath, uint currentProcessId, IVelopackLogger? logger) 
            : base(currentProcessPath, currentProcessId, logger)
        {
        }

        public override string? AppId
        {
            get => _appId ?? base.AppId;
        }

        public void UpdateAppId(string? appId)
        {
            _appId = appId;
        }

        public override SemanticVersion? CurrentlyInstalledVersion
        {
            get => _currentlyInstalledVersion ?? base.CurrentlyInstalledVersion;
        }

        public void UpdateCurrentlyInstalledVersion(SemanticVersion? version)
        {
            _currentlyInstalledVersion = version;
        }

        public override string? PackagesDir
        {
            get => _packagesDir ?? base.PackagesDir;
        }

        public void UpdatePackagesDir(string? packagesDir)
        {
            _packagesDir = packagesDir;
        }


        public override string? UpdateExePath
        {
            get => _updateExePath ?? base.UpdateExePath;
        }

        public void UpdateUpdateExePath(string? updateExePath)
        {
            _updateExePath = updateExePath;
        }
    }
}
