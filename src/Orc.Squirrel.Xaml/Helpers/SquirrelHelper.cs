﻿namespace Orc.Squirrel;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catel.IoC;
using Catel.Logging;
using Catel.Services;
using ViewModels;

public static class SquirrelHelper
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    [ObsoleteEx(ReplacementTypeOrMember = "Consider using Velopack", RemoveInVersion = "99.0", TreatAsErrorFromVersion = "99.0")]
    public static async Task<SquirrelLaunchResult> HandleSquirrelAutomaticallyAsync()
    {
        // Note: migrations to Squirrel should automatically be handled
        await VelopackHelper.HandleVelopackAutomaticallyAsync();

        Log.Debug("Handling squirrel");

        var application = Application.Current;
        if (application is null)
        {
            Log.Warning("Application is null, cannot handle squirrel");
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

        Log.Info("Application is started with squirrel argument '{0}', going to show message to user", lastArgument);

        var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
        var uiVisualizerService = dependencyResolver.ResolveRequired<IUIVisualizerService>();

        await uiVisualizerService.ShowDialogAsync<AppInstalledViewModel>();

        Log.Info("Closing application");

        application.Shutdown();

        return SquirrelLaunchResult.ClosingApplication;
    }
}
