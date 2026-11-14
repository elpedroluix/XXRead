using XXRead.UI.Platform;

namespace XXRead.Helpers.Services
{
	/// <summary>
	/// <see cref="IServiceAppSettings"/> stored in the MAUI Preferences (<see cref="AppSettings"/>, same keys as the XAML UI).
	/// </summary>
	public class ServiceAppSettingsPreferences : IServiceAppSettings
	{
		public bool FirstRun
		{
			get => AppSettings.FirstRun;
			set => AppSettings.FirstRun = value;
		}

		public bool DataSourceChanged
		{
			get => AppSettings.DataSourceChanged;
			set => AppSettings.DataSourceChanged = value;
		}

		public string ThemeMain
		{
			get => ToCssHex(AppSettings.ThemeMain);
			set => AppSettings.ThemeMain = value;
		}

		public string ThemePrimary
		{
			get => ToCssHex(AppSettings.ThemePrimary);
			set => AppSettings.ThemePrimary = value;
		}

		public string ThemeSecondary
		{
			get => ToCssHex(AppSettings.ThemeSecondary);
			set => AppSettings.ThemeSecondary = value;
		}

		public string ThemeFontPrimary
		{
			get => ToCssHex(AppSettings.ThemeFontPrimary);
			set => AppSettings.ThemeFontPrimary = value;
		}

		public string ThemeFontSecondary
		{
			get => ToCssHex(AppSettings.ThemeFontSecondary);
			set => AppSettings.ThemeFontSecondary = value;
		}

		/// <summary>
		/// The XAML UI stores MAUI hex colors ("#111", "#AARRGGBB"...) : the Blazor UI expects "#RRGGBB".
		/// </summary>
		private static string ToCssHex(string mauiColor)
		{
			return Color.FromArgb(mauiColor).ToRgbaHex();
		}
	}
}
