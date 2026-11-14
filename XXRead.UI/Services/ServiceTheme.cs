using MudBlazor;
using MudBlazor.Utilities;
using XXRead.UI.Platform;

namespace XXRead.UI.Services
{
	public class ServiceTheme : IServiceTheme
	{
		private const double DARK_LUMINANCE_THRESHOLD = 0.5;

		private static readonly string[] FONT_FAMILY = { "OpenSans", "sans-serif" };

		private readonly IServiceAppSettings _serviceAppSettings;

		public MudTheme Theme { get; private set; }

		public bool IsDarkMode { get; private set; }

		public string StatusBarColor { get; private set; }

		public event EventHandler ThemeChanged;

		public ServiceTheme(IServiceAppSettings serviceAppSettings)
		{
			_serviceAppSettings = serviceAppSettings;

			Build();
		}

		public void Reload()
		{
			Build();

			ThemeChanged?.Invoke(this, EventArgs.Empty);
		}

		private void Build()
		{
			// Same colors in both palettes : the user picks every color, IsDarkMode only tells MudBlazor which one to use
			var paletteLight = new PaletteLight();
			var paletteDark = new PaletteDark();
			ApplyColors(paletteLight);
			ApplyColors(paletteDark);

			Theme = new MudTheme()
			{
				PaletteLight = paletteLight,
				PaletteDark = paletteDark,
				Typography = new Typography()
				{
					Default = new DefaultTypography() { FontFamily = FONT_FAMILY },
				},
			};

			IsDarkMode = GetLuminance(_serviceAppSettings.ThemePrimary) < DARK_LUMINANCE_THRESHOLD;
			StatusBarColor = _serviceAppSettings.ThemeMain;
		}

		private void ApplyColors(Palette palette)
		{
			palette.Primary = _serviceAppSettings.ThemeMain;
			palette.AppbarBackground = _serviceAppSettings.ThemeMain;
			palette.AppbarText = _serviceAppSettings.ThemeFontPrimary;

			palette.Background = _serviceAppSettings.ThemePrimary;
			palette.Surface = _serviceAppSettings.ThemeSecondary;
			palette.DrawerBackground = _serviceAppSettings.ThemeSecondary;

			palette.TextPrimary = _serviceAppSettings.ThemeFontPrimary;
			palette.TextSecondary = _serviceAppSettings.ThemeFontSecondary;
			palette.DrawerText = _serviceAppSettings.ThemeFontPrimary;
		}

		/// <summary>
		/// Relative luminance (0 = black, 1 = white), as defined by WCAG.
		/// </summary>
		private static double GetLuminance(string color)
		{
			var mudColor = new MudColor(color);

			return 0.2126 * ToLinear(mudColor.R) + 0.7152 * ToLinear(mudColor.G) + 0.0722 * ToLinear(mudColor.B);
		}

		private static double ToLinear(byte channel)
		{
			double value = channel / 255.0;

			return value <= 0.03928 ? value / 12.92 : Math.Pow((value + 0.055) / 1.055, 2.4);
		}
	}
}
