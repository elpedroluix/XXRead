using MudBlazor;

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

		event EventHandler ThemeChanged;

		/// <summary>
		/// Rebuilds the theme from the settings (after a color change) and raises <see cref="ThemeChanged"/>.
		/// </summary>
		void Reload();
	}
}
