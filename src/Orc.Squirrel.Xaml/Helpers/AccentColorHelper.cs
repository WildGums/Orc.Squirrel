// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccentColorHelper.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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
            if (application == null)
            {
                return brush;
            }

            var accentBrush =  application.TryFindResource("AccentColorBrush") as SolidColorBrush;
            if (accentBrush != null)
            {
                brush = accentBrush;
            }

            return brush;
        }
    }
}