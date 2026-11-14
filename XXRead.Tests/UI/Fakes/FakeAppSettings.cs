using XXRead.UI.Platform;
using XXRead.UI.Theming;

namespace XXRead.Tests.UI.Fakes
{
	public class FakeAppSettings : IServiceAppSettings
	{
		public bool FirstRun { get; set; }
		public bool DataSourceChanged { get; set; }
		public string ThemeMain { get; set; } = ThemeColors.MainRed;
		public string ThemePrimary { get; set; } = ThemeColors.DarkPrimary;
		public string ThemeSecondary { get; set; } = ThemeColors.DarkSecondary;
		public string ThemeFontPrimary { get; set; } = ThemeColors.FontLightPrimary;
		public string ThemeFontSecondary { get; set; } = ThemeColors.FontLightSecondary;
	}
}
