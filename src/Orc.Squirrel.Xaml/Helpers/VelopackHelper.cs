namespace Orc.Squirrel;

using System.Threading.Tasks;
using Catel.Logging;
using global::Velopack;

public static class VelopackHelper
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public static async Task<SquirrelLaunchResult> HandleVelopackAutomaticallyAsync()
    {
        Log.Debug("Handling velopack");

        // Note: no need to display the UI, velopack will run the app automatically,
        // and we want to disturb users as little as possible.
        VelopackApp.Build()
            .Run();

        // Always return unhandled, build.run will automatically exit if needed
        return SquirrelLaunchResult.Unhandled;
    }
}
