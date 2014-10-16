// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateChannel.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2014 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
    /// <summary>
    /// Update channel model.
    /// </summary>
    public class UpdateChannel
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the default URL.
        /// </summary>
        /// <value>The default URL.</value>
        public string DefaultUrl { get; private set; }
    }
}