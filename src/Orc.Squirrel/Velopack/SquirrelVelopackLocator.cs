namespace Orc.Squirrel.Velopack
{
    using global::Velopack.Locators;
    using Microsoft.Extensions.Logging;
    using NuGet.Versioning;

    internal class SquirrelVelopackLocator : WindowsVelopackLocator
    {
        private string? _appId;
        private SemanticVersion? _currentlyInstalledVersion;

        public SquirrelVelopackLocator(ILogger logger) 
            : base(logger)
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
    }
}
