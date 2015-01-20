// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
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

        public MainViewModel(IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => uiVisualizerService);

            _uiVisualizerService = uiVisualizerService;

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
            _uiVisualizerService.ShowDialog<AppInstalledViewModel>();
        }
        #endregion

        protected override async Task Initialize()
        {
            await base.Initialize();
        }
    }
}