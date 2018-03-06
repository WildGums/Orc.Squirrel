[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]


public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.Squirrel
{
    
    public interface IUpdateService
    {
        Orc.Squirrel.UpdateChannel[] AvailableChannels { get; }
        bool CheckForUpdates { get; set; }
        Orc.Squirrel.UpdateChannel CurrentChannel { get; set; }
        bool IsUpdatedInstalled { get; }
        bool IsUpdateOutsideMaintenanceAvailable { get; }
        bool IsUpdateSystemAvailable { get; }
        public event System.EventHandler<System.EventArgs> UpdateInstalled;
        public event System.EventHandler<System.EventArgs> UpdateOutsideMaintenanceAvailable;
        System.Threading.Tasks.Task HandleUpdatesAsync(System.Nullable<System.DateTime> maximumReleaseDate = null);
        void Initialize(System.Collections.Generic.IEnumerable<Orc.Squirrel.UpdateChannel> availableChannels, Orc.Squirrel.UpdateChannel defaultChannel, bool defaultCheckForUpdatesValue);
    }
    public class static Settings
    {
        public class static Application
        {
            public class static AutomaticUpdates
            {
                public const string CheckForUpdates = "AutomaticUpdates.CheckForUpdates";
                public const string UpdateChannel = "AutomaticUpdates.UpdateChannel";
                public static string GetChannelSettingName(string channelName) { }
            }
        }
    }
    public class static SquirrelArguments
    {
        public const string FirstRun = "--squirrel-firstrun";
        public const string Install = "--squirrel-install";
        public const string Uninstall = "--squirrel-uninstall";
        public const string Updated = "--squirrel-updated";
        public static bool IsSquirrelArgument(string argument) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{Name} => {DefaultUrl}")]
    public class UpdateChannel
    {
        public UpdateChannel(string name, string defaultUrl) { }
        public string DefaultUrl { get; }
        public string Description { get; set; }
        public bool IsPrerelease { get; set; }
        public string Name { get; }
        public override string ToString() { }
    }
    public class UpdateService : Orc.Squirrel.IUpdateService
    {
        public UpdateService(Catel.Configuration.IConfigurationService configurationService, Orc.FileSystem.IFileService fileService) { }
        public Orc.Squirrel.UpdateChannel[] AvailableChannels { get; }
        public bool CheckForUpdates { get; set; }
        public Orc.Squirrel.UpdateChannel CurrentChannel { get; set; }
        public bool IsUpdatedInstalled { get; }
        public bool IsUpdateOutsideMaintenanceAvailable { get; }
        public bool IsUpdateSystemAvailable { get; }
        public event System.EventHandler<System.EventArgs> UpdateInstalled;
        public event System.EventHandler<System.EventArgs> UpdateOutsideMaintenanceAvailable;
        public System.Threading.Tasks.Task HandleUpdatesAsync(System.Nullable<System.DateTime> maximumReleaseDate = null) { }
        public void Initialize(System.Collections.Generic.IEnumerable<Orc.Squirrel.UpdateChannel> availableChannels, Orc.Squirrel.UpdateChannel defaultChannel, bool defaultCheckForUpdatesValue) { }
    }
}