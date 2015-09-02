// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateChannel.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
    using System.Diagnostics;
    using Catel;

    /// <summary>
    /// Update channel model.
    /// </summary>
    [DebuggerDisplay("{Name} => {DefaultUrl}")]
    public class UpdateChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateChannel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultUrl">The default URL.</param>
        public UpdateChannel(string name, string defaultUrl)
        {
            Argument.IsNotNull(() => name);
            Argument.IsNotNull(() => defaultUrl);

            Name = name;
            Description = string.Empty;
            DefaultUrl = defaultUrl;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the default URL.
        /// </summary>
        /// <value>The default URL.</value>
        public string DefaultUrl { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this channel is a prerelease channel.
        /// </summary>
        /// <value><c>true</c> if this channel is a prerelease channel; otherwise, <c>false</c>.</value>
        public bool IsPrerelease { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}