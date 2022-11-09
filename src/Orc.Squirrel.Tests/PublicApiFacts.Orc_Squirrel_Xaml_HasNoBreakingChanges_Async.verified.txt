[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Orc.Squirrel.Tests")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName="")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/squirrel", "Orc.Squirrel.Views")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.wildgums.com/orc/squirrel", "orcsquirrel")]
[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.Squirrel
{
    public static class SquirrelHelper
    {
        public static System.Threading.Tasks.Task<Orc.Squirrel.SquirrelLaunchResult> HandleSquirrelAutomaticallyAsync() { }
    }
    public enum SquirrelLaunchResult
    {
        Unhandled = 0,
        ClosingApplication = 1,
    }
}
namespace Orc.Squirrel.ViewModels
{
    public class AppInstalledViewModel : Catel.MVVM.ViewModelBase
    {
        public static readonly Catel.Data.IPropertyData AppIconProperty;
        public static readonly Catel.Data.IPropertyData AppNameProperty;
        public static readonly Catel.Data.IPropertyData AppVersionProperty;
        public AppInstalledViewModel(Catel.Services.IProcessService processService, Catel.Services.IDispatcherService dispatcherService, Catel.Services.ILanguageService languageService) { }
        public System.Windows.Media.Imaging.BitmapSource? AppIcon { get; }
        public string AppName { get; }
        public string AppVersion { get; }
        public Catel.MVVM.Command RunApplication { get; }
    }
}
namespace Orc.Squirrel.Views
{
    public class AppInstalledWindow : Catel.Windows.DataWindow, System.Windows.Markup.IComponentConnector
    {
        public AppInstalledWindow() { }
        public AppInstalledWindow(Orc.Squirrel.ViewModels.AppInstalledViewModel? viewModel) { }
        public void InitializeComponent() { }
    }
}