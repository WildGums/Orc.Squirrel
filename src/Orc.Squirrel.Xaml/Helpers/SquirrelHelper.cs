// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SquirrelHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
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

        public static async Task<SquirrelLaunchResult> HandleSquirrelAutomaticallyAsync()
        {
            Log.Debug("Handling squirrel");

            var application = Application.Current;
            if (application == null)
            {
                Log.Warning("Application is null, cannot handle squirrel");
                return SquirrelLaunchResult.Unhandled;
            }

            var arguments = Environment.GetCommandLineArgs();
            var lastArgument = arguments.LastOrDefault();
            if (!string.IsNullOrWhiteSpace(lastArgument))
            {
                if (SquirrelArguments.IsSquirrelArgument(lastArgument))
                {
                    Log.Info("Application is started with squirrel argument '{0}', going to show message to user", lastArgument);

                    var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
                    var uiVisualizerService = dependencyResolver.Resolve<IUIVisualizerService>();

                    await uiVisualizerService.ShowDialogAsync<AppInstalledViewModel>();

                    Log.Info("Closing application");

                    application.Shutdown();
                    return SquirrelLaunchResult.ClosingApplication;
                }
            }

            return SquirrelLaunchResult.Unhandled;
        }
    }
}