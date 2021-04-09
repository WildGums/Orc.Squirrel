// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccentColorHelper.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.Squirrel
{
    using System.Windows;
    using System.Windows.Media;

    public static class AccentColorHelper
    {
        public static SolidColorBrush GetAccentColor()
        {
            var brush = new SolidColorBrush(Color.FromArgb(255, 58, 202, 116));

            var application = Application.Current;
            if (application is null)
            {
                return brush;
            }

            var accentBrush =  application.TryFindResource("AccentColorBrush") as SolidColorBrush;
            if (accentBrush is not null)
            {
                brush = accentBrush;
            }

            return brush;
        }
    }
}
