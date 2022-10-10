namespace Orc.Squirrel
{
    /// <summary>
    /// Squirrel result enum.
    /// </summary>
    public enum SquirrelLaunchResult
    {
        /// <summary>
        /// Unhandled, meaning no interaction took place.
        /// </summary>
        Unhandled = 0,

        /// <summary>
        /// Squirrel has handled the application is closing it.
        /// </summary>
        ClosingApplication = 1,
    }
}
