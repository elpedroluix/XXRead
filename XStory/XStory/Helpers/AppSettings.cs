using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XStory.Helpers
{
    public static class AppSettings
    {
        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }

        public static string ThemeFontPrimary
        {
            get => Preferences.Get(nameof(ThemeFontPrimary), Themes.Theme.FontLightPrimary);
            set => Preferences.Set(nameof(ThemeFontPrimary), value);
        }

        public static string ThemeFontSecondary
        {
            get => Preferences.Get(nameof(ThemeFontSecondary), Themes.Theme.FontLightSecondary);
            set => Preferences.Set(nameof(ThemeFontSecondary), value);
        }

        public static string ThemePrimary
        {
            get => Preferences.Get(nameof(ThemePrimary), Themes.Theme.DarkPrimary);
            set => Preferences.Set(nameof(ThemePrimary), value);
        }

        public static string ThemeSecondary
        {
            get => Preferences.Get(nameof(ThemeSecondary), Themes.Theme.DarkSecondary);
            set => Preferences.Set(nameof(ThemeSecondary), value);
        }

        public static string ThemeMain
        {
            get => Preferences.Get(nameof(ThemeMain), Themes.Theme.MainRed);
            set => Preferences.Set(nameof(ThemeMain), value);
        }
    }
}
