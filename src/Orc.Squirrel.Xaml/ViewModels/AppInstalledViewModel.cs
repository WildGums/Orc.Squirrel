// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInstalledViewModel.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel.ViewModels
{
    using System.Reflection;
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
            var appVersion = _entryAssembly.InformationalVersion();

            Title = string.Format("{0} v{1} is installed", appName, appVersion);
            AppName = appName;
            AppVersion = appVersion;

            Description = string.Format("{0} is successfully installed. You can now run the application via the shortcut that is created on the desktop.",
                appName);
        }

        #region Properties
        public string AppName { get; private set; }

        public string AppVersion { get; private set; }

        public string Description { get; private set; }
        #endregion

        #region Commands
        public Command RunApplication { get; private set; }

        private void OnRunApplicationExecute()
        {
            _processService.StartProcess(_entryAssembly.Location);
        }
        #endregion

        #region Methods
        #endregion
    }
}