namespace Orc.Squirrel
{
    /// <summary>
    /// Settings class containing the settings information.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Application settings.
        /// </summary>
        public static class Application
        {
            /// <summary>
            /// Automatic update settings.
            /// </summary>
            public static class AutomaticUpdates
            {
                /// <summary>
                /// The check for updates settings key.
                /// </summary>
                public const string CheckForUpdates = "AutomaticUpdates.CheckForUpdates";

                /// <summary>
                /// The update channel settings key.
                /// </summary>
                public const string UpdateChannel = "AutomaticUpdates.UpdateChannel";

                /// <summary>
                /// Gets the name of the channel setting.
                /// </summary>
                /// <param name="channelName">Name of the channel.</param>
                /// <returns>System.String.</returns>
                public static string GetChannelSettingName(string channelName)
                {
                    return string.Format("AutomaticUpdates.Channels.{0}", channelName);
                }
            }
        }
    }

    /// <summary>
    /// Class containing squirrel constants.
    /// </summary>
    public static class SquirrelArguments
    {
        /// <summary>
        /// The application is ran for the first time after installation.
        /// </summary>
        public const string FirstRun = "--squirrel-firstrun";

        /// <summary>
        /// The install is installed.
        /// </summary>
        public const string Install = "--squirrel-install";

        /// <summary>
        /// The install is uninstalled.
        /// </summary>
        public const string Uninstall = "--squirrel-uninstall";

        /// <summary>
        /// The application is updated.
        /// </summary>
        public const string Updated = "--squirrel-updated";

        /// <summary>
        /// Determines whether the specified argument is a known squirrel argument.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns><c>true</c> if the argument is a known squirrel argument; otherwise, <c>false</c>.</returns>
        public static bool IsSquirrelArgument(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                return false;
            }

            return argument.ToLower().Contains("--squirrel-");
        }
    }
}
