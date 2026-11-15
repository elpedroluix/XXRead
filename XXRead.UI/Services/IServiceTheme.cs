using MudBlazor;
using XXRead.UI.Theming;

namespace XXRead.UI.Services
{
	/// <summary>
	/// Builds the <see cref="MudTheme"/> from the colors stored in <see cref="Platform.IServiceAppSettings"/>.
	/// </summary>
	public interface IServiceTheme
	{
		MudTheme Theme { get; }

		/// <summary>
		/// True when the background is dark : MudBlazor then derives its hover and divider colors for a dark background.
		/// </summary>
		bool IsDarkMode { get; }

		string StatusBarColor { get; }

		/// <summary>
		/// Color behind the navigation bar / home handle : the page background, so that the bar blends in.
		/// </summary>
		string NavigationBarColor { get; }

		event EventHandler ThemeChanged;

		/// <summary>
		/// Rebuilds the theme from the settings (after a color change) and raises <see cref="ThemeChanged"/>.
		/// </summary>
		void Reload();

		/// <summary>
		/// Saves the background and font colors in the settings, then applies them (<see cref="Reload"/>).
		/// </summary>
		void SetBackground(ThemeBackground background);

		/// <summary>
		/// Saves the main color (AppBar, buttons, status bar) in the settings, then applies it (<see cref="Reload"/>).
		/// </summary>
		void SetMainColor(string color);
	}
}
