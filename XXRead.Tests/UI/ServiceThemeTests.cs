using XXRead.UI.Platform;
using XXRead.UI.Services;
using XXRead.UI.Theming;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class ServiceThemeTests
	{
		[TestMethod]
		public void DarkSettings_DarkModeWithSettingsColors()
		{
			var serviceTheme = new ServiceTheme(new FakeAppSettings());

			Assert.IsTrue(serviceTheme.IsDarkMode);
			Assert.AreEqual(ThemeColors.MainRed, serviceTheme.StatusBarColor);
			Assert.AreEqual(ThemeColors.MainRed.ToLower(), serviceTheme.Theme.PaletteDark.Primary.ToString(MudBlazor.Utilities.MudColorOutputFormats.Hex));
			Assert.AreEqual(ThemeColors.DarkPrimary.ToLower(), serviceTheme.Theme.PaletteDark.Background.ToString(MudBlazor.Utilities.MudColorOutputFormats.Hex));
		}

		[TestMethod]
		public void LightSettings_LightMode()
		{
			var settings = new FakeAppSettings() { ThemePrimary = ThemeColors.LightPrimary };

			var serviceTheme = new ServiceTheme(settings);

			Assert.IsFalse(serviceTheme.IsDarkMode);
		}

		[TestMethod]
		public void Reload_ReadsSettingsAgainAndRaisesThemeChanged()
		{
			var settings = new FakeAppSettings();
			var serviceTheme = new ServiceTheme(settings);
			bool raised = false;
			serviceTheme.ThemeChanged += (sender, e) => raised = true;

			settings.ThemePrimary = ThemeColors.LightPrimary;
			serviceTheme.Reload();

			Assert.IsTrue(raised);
			Assert.IsFalse(serviceTheme.IsDarkMode);
		}

		private class FakeAppSettings : IServiceAppSettings
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
}
