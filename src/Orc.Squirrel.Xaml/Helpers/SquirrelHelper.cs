namespace Orc.Squirrel;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catel.IoC;
using Catel.Logging;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewModels;

public static class SquirrelHelper
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(SquirrelHelper));

    [ObsoleteEx(ReplacementTypeOrMember = "Consider using Velopack", RemoveInVersion = "99.0", TreatAsErrorFromVersion = "99.0")]
    public static async Task<SquirrelLaunchResult> HandleSquirrelAutomaticallyAsync()
    {
        // Note: migrations to Squirrel should automatically be handled
        await VelopackHelper.HandleVelopackAutomaticallyAsync();

        Logger.LogDebug("Handling squirrel");

        var application = Application.Current;
        if (application is null)
        {
            Logger.LogWarning("Application is null, cannot handle squirrel");
            return SquirrelLaunchResult.Unhandled;
        }

        var arguments = Environment.GetCommandLineArgs();
        var lastArgument = arguments.LastOrDefault();
        if (string.IsNullOrWhiteSpace(lastArgument))
        {
            return SquirrelLaunchResult.Unhandled;
        }

        if (!SquirrelArguments.IsSquirrelArgument(lastArgument))
        {
            return SquirrelLaunchResult.Unhandled;
        }

        Logger.LogInformation("Application is started with squirrel argument '{0}', going to show message to user", lastArgument);

        var serviceProvider = IoCContainer.ServiceProvider;
        var uiVisualizerService = serviceProvider.GetRequiredService<IUIVisualizerService>();

        await uiVisualizerService.ShowDialogAsync<AppInstalledViewModel>();

        Logger.LogInformation("Closing application");

        application.Shutdown();

        return SquirrelLaunchResult.ClosingApplication;
    }
}
