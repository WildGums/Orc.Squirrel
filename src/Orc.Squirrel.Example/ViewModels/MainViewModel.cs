// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel.Example.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Catel.Services;
    using Squirrel.ViewModels;

    public class MainViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IDispatcherService _dispatcherService;

        public MainViewModel(IUIVisualizerService uiVisualizerService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => dispatcherService);

            _uiVisualizerService = uiVisualizerService;
            _dispatcherService = dispatcherService;

            ShowInstalledDialog = new Command(OnShowInstalledDialogExecute);
        }

        #region Properties
        public override string Title
        {
            get { return "Squirrel example"; }
        }
        #endregion

        #region Commands
        public Command ShowInstalledDialog { get; private set; }

        private void OnShowInstalledDialogExecute()
        {
            _dispatcherService.BeginInvoke(async () =>
            {
                _uiVisualizerService.ShowDialog<AppInstalledViewModel>();
                await CloseViewModelAsync(null);
            });
        }
        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }
    }
}