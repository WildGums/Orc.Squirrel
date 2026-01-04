namespace Orc.Squirrel;

using System.Threading.Tasks;
using Catel.Logging;
using global::Velopack;
using Microsoft.Extensions.Logging;

public static class VelopackHelper
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(VelopackHelper));

    public static async Task<SquirrelLaunchResult> HandleVelopackAutomaticallyAsync()
    {
        Logger.LogDebug("Handling velopack");

        // Note: no need to display the UI, velopack will run the app automatically,
        // and we want to disturb users as little as possible.
        VelopackApp.Build()
            .Run();

        // Always return unhandled, build.run will automatically exit if needed
        return SquirrelLaunchResult.Unhandled;
    }
}
