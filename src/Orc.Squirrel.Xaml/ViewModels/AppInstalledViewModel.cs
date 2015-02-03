// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInstalledViewModel.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel.ViewModels
{
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Catel;
    using Catel.MVVM;
    using Catel.Reflection;
    using Catel.Services;

    public class AppInstalledViewModel : ViewModelBase
    {
        private readonly IProcessService _processService;

        private readonly Assembly _entryAssembly = AssemblyHelper.GetEntryAssembly();

        public AppInstalledViewModel(IProcessService processService)
        {
            Argument.IsNotNull(() => processService);

            _processService = processService;

            RunApplication = new Command(OnRunApplicationExecute);

            var appName = _entryAssembly.Title();
            var appVersion = _entryAssembly.InformationalVersion() ?? _entryAssembly.Version();

            Title = string.Format("{0} v{1} is installed", appName, appVersion);

            AppIcon = ExtractLargestIcon();
            AppName = appName;
            AppVersion = appVersion;
            AccentColorBrush = AccentColorHelper.GetAccentColor();
        }

        #region Properties
        public string AppName { get; private set; }

        public string AppVersion { get; private set; }

        public BitmapSource AppIcon { get; private set; }

        public SolidColorBrush AccentColorBrush { get; private set; }
        #endregion

        #region Commands
        public Command RunApplication { get; private set; }

        private void OnRunApplicationExecute()
        {
            _processService.StartProcess(_entryAssembly.Location);
        }
        #endregion

        #region Methods
        private BitmapImage ExtractLargestIcon()
        {
            return IconHelper.ExtractLargestIconFromFile(_entryAssembly.Location);
        }
        #endregion
    }
}