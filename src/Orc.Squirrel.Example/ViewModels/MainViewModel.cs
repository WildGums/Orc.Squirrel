namespace Orc.Squirrel.Example.ViewModels;

using System;
using System.Threading.Tasks;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Squirrel;
using Services;
using Squirrel.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly IDispatcherService _dispatcherService;
    private readonly IUpdateService _updateService;
    private readonly IUpdateExecutableLocationService _updateExecutableLocationService;
    private TaskCommand _checkForUpdates;
    private TaskCommand _update;
    private Command _showInstalledDialog;
    private readonly string _title;

    public MainViewModel(IUIVisualizerService uiVisualizerService, IDispatcherService dispatcherService,
        IUpdateService updateService, IUpdateExecutableLocationService updateExecutableLocationService,
        ILanguageService languageService, IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _uiVisualizerService = uiVisualizerService;
        _dispatcherService = dispatcherService;
        _updateService = updateService;
        _updateExecutableLocationService = updateExecutableLocationService;
        _title = languageService.GetRequiredString("Orc_Squirrel_Example_MainViewModel_Title");

        InitializeCommands(serviceProvider);
        InitializeDebugValues();
    }

    public override string Title
    {
        get { return _title; }
    }

    public bool IsInstallingUpdate { get; private set; }

    public bool IsUpdateAvailable { get; private set; }

    public string UpdateUrl { get; set; }

    public string ExecutableFileName { get; set; }

    public int Progress { get; set; }

    public TaskCommand CheckForUpdates => _checkForUpdates;

    public TaskCommand Update => _update;

    public Command ShowInstalledDialog => _showInstalledDialog;

    private void InitializeCommands(IServiceProvider serviceProvider)
    {
        _checkForUpdates = new TaskCommand(serviceProvider, OnCheckForUpdatesExecuteAsync, OnCheckForUpdatesCanExecute);
        _update = new TaskCommand(serviceProvider, OnUpdateExecuteAsync, OnUpdateCanExecute);
        _showInstalledDialog = new Command(serviceProvider, OnShowInstalledDialogExecute);
    }

    private void InitializeDebugValues()
    {
#if DEBUG
        UpdateUrl = "https://downloads.wildgums.com/flexgrid/alpha";
        ExecutableFileName = Environment.ExpandEnvironmentVariables("%localappdata%\\WildGums\\Flex Grid_alpha\\FlexGrid.exe");
#endif
    }

    private bool OnCheckForUpdatesCanExecute()
    {
        if (string.IsNullOrWhiteSpace(UpdateUrl))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(ExecutableFileName);
    }

    private async Task OnCheckForUpdatesExecuteAsync()
    {
        await UpdateCustomChannelsAsync();

        var result = await _updateService.CheckForUpdatesAsync(new SquirrelContext());
        IsUpdateAvailable = result.IsUpdateInstalledOrAvailable;
    }

    private bool OnUpdateCanExecute()
    {
        if (string.IsNullOrWhiteSpace(UpdateUrl))
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(ExecutableFileName);
    }

    private async Task OnUpdateExecuteAsync()
    {
        await UpdateCustomChannelsAsync();

        try
        {
            IsInstallingUpdate = true;

            await _updateService.InstallAvailableUpdatesAsync(new SquirrelContext());
        }
        finally
        {
            Progress = 0;

            IsInstallingUpdate = false;
        }
    }

    private void OnShowInstalledDialogExecute()
    {
        // Dispatch since we close the vm
        _dispatcherService.BeginInvoke(async () =>
        {
            await _uiVisualizerService.ShowDialogAsync<AppInstalledViewModel>();
            await CloseViewModelAsync(null);
        });
    }

    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _updateService.UpdateProgress += OnUpdateServiceProgress;
    }

    protected override async Task CloseAsync()
    {
        _updateService.UpdateProgress -= OnUpdateServiceProgress;

        await base.CloseAsync();
    }

    private void OnUpdateServiceProgress(object sender, SquirrelProgressEventArgs e)
    {
        Progress = e.Percentage;
    }

    private async Task UpdateCustomChannelsAsync()
    {
        var channels = new UpdateChannel[]
        {
            new("Custom", UpdateUrl)
            {
                IsPrerelease = true
            }
        };

        await _updateService.InitializeAsync(channels, channels[0], true);
    }

    private void OnExecutableFileNameChanged()
    {
        ((ExampleUpdateExecutableLocationService)_updateExecutableLocationService).ExecutableFileName = ExecutableFileName;
    }
}
