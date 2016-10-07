// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SquirrelHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
    using System;
    using System.Linq;
    using System.Windows;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Services;
    using ViewModels;

    public static class SquirrelHelper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static SquirrelResult HandleSquirrelAutomatically()
        {
            Log.Debug("Handling squirrel");

            var application = Application.Current;
            if (application == null)
            {
                Log.Warning("Application is null, cannot handle squirrel");
                return SquirrelResult.Unhandled;
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

                    uiVisualizerService.ShowDialog<AppInstalledViewModel>();

                    Log.Info("Closing application");

                    application.Shutdown();
                    return SquirrelResult.ClosingApplication;
                }
            }

            return SquirrelResult.Unhandled;
        }
    }
}