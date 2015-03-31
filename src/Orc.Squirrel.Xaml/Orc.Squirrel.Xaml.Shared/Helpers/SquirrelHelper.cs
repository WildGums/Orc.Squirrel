// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SquirrelHelper.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
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

        public static void HandleSquirrelAutomatically()
        {
            Log.Debug("Handling squirrel");

            var application = Application.Current;
            if (application == null)
            {
                Log.Warning("Application is null, cannot handle squirrel");
                return;
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
                }
            }
        }
    }
}