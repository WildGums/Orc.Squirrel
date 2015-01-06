// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orcomp.Squirrel
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
}