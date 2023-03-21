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
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly IDispatcherService _dispatcherService;
    private readonly IUpdateService _updateService;
    private readonly IUpdateExecutableLocationService _updateExecutableLocationService;

    public MainViewModel(IUIVisualizerService uiVisualizerService, IDispatcherService dispatcherService,
        IUpdateService updateService, IUpdateExecutableLocationService updateExecutableLocationService)
    {
        ArgumentNullException.ThrowIfNull(uiVisualizerService);
        ArgumentNullException.ThrowIfNull(dispatcherService);
        ArgumentNullException.ThrowIfNull(updateService);

        _uiVisualizerService = uiVisualizerService;
        _dispatcherService = dispatcherService;
        _updateService = updateService;
        _updateExecutableLocationService = updateExecutableLocationService;
        CheckForUpdates = new TaskCommand(OnCheckForUpdatesExecuteAsync, OnCheckForUpdatesCanExecute);
        Update = new TaskCommand(OnUpdateExecuteAsync, OnUpdateCanExecute);
        ShowInstalledDialog = new Command(OnShowInstalledDialogExecute);

        Title = "Orc.Squirrel example";

#if DEBUG
        UpdateUrl = "https://downloads.wildgums.com/flexgrid/alpha";
        ExecutableFileName = Environment.ExpandEnvironmentVariables("%localappdata%\\WildGums\\Flex Grid_alpha\\FlexGrid.exe");
#endif
    }

    public override string Title
    {
        get { return "Squirrel example"; }
    }

    public bool IsInstallingUpdate { get; private set; }

    public bool IsUpdateAvailable { get; private set; }

    public string UpdateUrl { get; set; }

    public string ExecutableFileName { get; set; }

    public int Progress { get; set; }

    public TaskCommand CheckForUpdates { get; }

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

    public TaskCommand Update { get; }

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

    public Command ShowInstalledDialog { get; }

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
