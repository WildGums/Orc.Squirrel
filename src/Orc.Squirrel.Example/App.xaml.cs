namespace Orc.Squirrel.Example
{
    using System.Globalization;
    using System.Windows;
    using Catel.IoC;
    using Catel.Services;
    using Orc.Squirrel.Example.Services;
    using Orchestra;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceLocator = ServiceLocator.Default;

            serviceLocator.RegisterType<IUpdateExecutableLocationService, ExampleUpdateExecutableLocationService>();

            // Note: it's best to use .CurrentUICulture in actual apps since it will use the preferred language
            // of the user. But in order to demo multilingual features for devs (who mostly have en-US as .CurrentUICulture),
            // we use .CurrentCulture for the sake of the demo
            var languageService = serviceLocator.ResolveType<ILanguageService>();
            languageService.PreferredCulture = CultureInfo.CurrentCulture;
            languageService.FallbackCulture = new CultureInfo("en-US");

            this.ApplyTheme();

            base.OnStartup(e);
        }
    }
}
