// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppInstalledWindow.xaml.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel.Views
{
    using Catel.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for AppInstalledWindow.xaml.
    /// </summary>
    public partial class AppInstalledWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInstalledWindow"/> class.
        /// </summary>
        public AppInstalledWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInstalledWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to inject.</param>
        /// <remarks>
        /// This constructor can be used to use view-model injection.
        /// </remarks>
        public AppInstalledWindow(AppInstalledViewModel viewModel)
            : base(viewModel, DataWindowMode.Close)
        {
            InitializeComponent();
        }
    }
}