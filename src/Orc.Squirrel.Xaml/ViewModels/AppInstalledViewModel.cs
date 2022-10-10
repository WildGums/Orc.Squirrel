namespace Orc.Squirrel.ViewModels
{
    using System;
    using System.Reflection;
    using System.Windows.Media.Imaging;
    using Catel.MVVM;
    using Catel.Reflection;
    using Catel.Services;
    using Orc.Theming;

    public class AppInstalledViewModel : ViewModelBase
    {
        private readonly IProcessService _processService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ILanguageService _languageService;
        private readonly Assembly _entryAssembly = AssemblyHelper.GetRequiredEntryAssembly();

        public AppInstalledViewModel(IProcessService processService, IDispatcherService dispatcherService, ILanguageService languageService)
        {
            ArgumentNullException.ThrowIfNull(processService);
            ArgumentNullException.ThrowIfNull(dispatcherService);
            ArgumentNullException.ThrowIfNull(languageService);

            _processService = processService;
            _dispatcherService = dispatcherService;
            _languageService = languageService;
            
            RunApplication = new Command(OnRunApplicationExecute);

            var appName = _entryAssembly.Title() ?? string.Empty;
            var appVersion = _entryAssembly.InformationalVersion() ?? _entryAssembly.Version();

            Title = string.Format(languageService.GetRequiredString("Squirrel_AppInstalled"), appName, appVersion);

            AppIcon = ExtractLargestIcon();
            AppName = appName;
            AppVersion = appVersion;
        }

        public string AppName { get; private set; }

        public string AppVersion { get; private set; }

        public BitmapSource? AppIcon { get; private set; }

        public Command RunApplication { get; private set; }

        private void OnRunApplicationExecute()
        {
            _dispatcherService.BeginInvoke(() =>
            {
                var location = _entryAssembly.Location;

                // Note: in .NET Core, entry assembly can be a .dll, make sure to enforce exe
                location = System.IO.Path.ChangeExtension(location, ".exe");

                _processService.StartProcess(location);

#pragma warning disable 4014
                CloseViewModelAsync(null);
#pragma warning restore 4014
            });
        }

        private BitmapImage? ExtractLargestIcon()
        {
            return IconHelper.ExtractLargestIconFromFile(_entryAssembly.Location);
        }
    }
}
