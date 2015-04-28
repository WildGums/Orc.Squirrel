// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SquirrelResult.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
    /// <summary>
    /// Squirrel result enum.
    /// </summary>
    public enum SquirrelResult
    {
        /// <summary>
        /// Unhandled, meaning no interaction took place.
        /// </summary>
        Unhandled,

        /// <summary>
        /// Squirrel has handled the application is closing it.
        /// </summary>
        ClosingApplication,
    }
}