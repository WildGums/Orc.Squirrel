[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Orc.Squirrel.Tests")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v3.1", FrameworkDisplayName="")]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.Squirrel
{
    public interface IUpdateExecutableLocationService
    {
        string FindUpdateExecutable();
        string GetApplicationExecutable();
    }
    public interface IUpdateService
    {
        Orc.Squirrel.UpdateChannel[] AvailableChannels { get; }
        bool CheckForUpdates { get; set; }
        Orc.Squirrel.UpdateChannel CurrentChannel { get; set; }
        bool IsUpdateSystemAvailable { get; }
        bool IsUpdatedInstalled { get; }
        event System.EventHandler<Orc.Squirrel.SquirrelEventArgs> UpdateInstalled;
        event System.EventHandler<Orc.Squirrel.SquirrelEventArgs> UpdateInstalling;
        event System.EventHandler<Orc.Squirrel.SquirrelProgressEventArgs> UpdateProgress;
        System.Threading.Tasks.Task<Orc.Squirrel.SquirrelResult> CheckForUpdatesAsync(Orc.Squirrel.SquirrelContext context);
        void Initialize(System.Collections.Generic.IEnumerable<Orc.Squirrel.UpdateChannel> availableChannels, Orc.Squirrel.UpdateChannel defaultChannel, bool defaultCheckForUpdatesValue);
        System.Threading.Tasks.Task<Orc.Squirrel.SquirrelResult> InstallAvailableUpdatesAsync(Orc.Squirrel.SquirrelContext context);
    }
    public static class Settings
    {
        public static class Application
        {
            public static class AutomaticUpdates
            {
                public const string CheckForUpdates = "AutomaticUpdates.CheckForUpdates";
                public const string UpdateChannel = "AutomaticUpdates.UpdateChannel";
                public static string GetChannelSettingName(string channelName) { }
            }
        }
    }
    public static class SquirrelArguments
    {
        public const string FirstRun = "--squirrel-firstrun";
        public const string Install = "--squirrel-install";
        public const string Uninstall = "--squirrel-uninstall";
        public const string Updated = "--squirrel-updated";
        public static bool IsSquirrelArgument(string argument) { }
    }
    public class SquirrelContext
    {
        public SquirrelContext() { }
        public string ChannelName { get; set; }
    }
    public class SquirrelEventArgs : System.EventArgs
    {
        public SquirrelEventArgs(Orc.Squirrel.SquirrelResult result) { }
        public Orc.Squirrel.SquirrelResult SquirrelResult { get; }
    }
    public class SquirrelProgressEventArgs
    {
        public SquirrelProgressEventArgs(int percentage) { }
        public int Percentage { get; }
    }
    public class SquirrelResult
    {
        public SquirrelResult() { }
        public string CurrentVersion { get; set; }
        public bool IsUpdateInstalledOrAvailable { get; set; }
        public string NewVersion { get; set; }
    }
    [System.Diagnostics.DebuggerDisplay("{Name} => {DefaultUrl}")]
    public class UpdateChannel
    {
        public UpdateChannel(string name, string defaultUrl) { }
        public string DefaultUrl { get; }
        public string Description { get; set; }
        public bool IsPrerelease { get; set; }
        public string Name { get; }
        public override string ToString() { }
    }
    public class UpdateExecutableLocationService : Orc.Squirrel.IUpdateExecutableLocationService
    {
        public UpdateExecutableLocationService(Orc.FileSystem.IFileService fileService) { }
        public string FindUpdateExecutable() { }
        public virtual string GetApplicationExecutable() { }
    }
    public class UpdateService : Orc.Squirrel.IUpdateService
    {
        public UpdateService(Catel.Configuration.IConfigurationService configurationService, Orc.FileSystem.IFileService fileService, Orc.Squirrel.IUpdateExecutableLocationService updateExecutableLocationService) { }
        public Orc.Squirrel.UpdateChannel[] AvailableChannels { get; }
        public bool CheckForUpdates { get; set; }
        public Orc.Squirrel.UpdateChannel CurrentChannel { get; set; }
        public bool IsUpdateSystemAvailable { get; }
        public bool IsUpdatedInstalled { get; }
        public event System.EventHandler<Orc.Squirrel.SquirrelEventArgs> UpdateInstalled;
        public event System.EventHandler<Orc.Squirrel.SquirrelEventArgs> UpdateInstalling;
        public event System.EventHandler<Orc.Squirrel.SquirrelProgressEventArgs> UpdateProgress;
        public System.Threading.Tasks.Task<Orc.Squirrel.SquirrelResult> CheckForUpdatesAsync(Orc.Squirrel.SquirrelContext context) { }
        protected string GetChannelUrl(Orc.Squirrel.SquirrelContext context) { }
        protected virtual string GetCurrentApplicationVersion() { }
        public void Initialize(System.Collections.Generic.IEnumerable<Orc.Squirrel.UpdateChannel> availableChannels, Orc.Squirrel.UpdateChannel defaultChannel, bool defaultCheckForUpdatesValue) { }
        public System.Threading.Tasks.Task<Orc.Squirrel.SquirrelResult> InstallAvailableUpdatesAsync(Orc.Squirrel.SquirrelContext context) { }
        protected virtual void RaiseProgressChanged(int percentage) { }
    }
}