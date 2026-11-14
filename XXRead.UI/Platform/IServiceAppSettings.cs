namespace XXRead.UI.Platform
{
	/// <summary>
	/// UI settings, implemented by each host (MAUI Preferences, JSON file for the WebHost).
	/// The data source is not here : it belongs to <see cref="XXRead.Core.Session.IServiceSettings"/>.
	/// Colors are CSS hex strings ("#RRGGBB").
	/// </summary>
	public interface IServiceAppSettings
	{
		bool FirstRun { get; set; }

		bool DataSourceChanged { get; set; }

		string ThemeMain { get; set; }

		string ThemePrimary { get; set; }

		string ThemeSecondary { get; set; }

		string ThemeFontPrimary { get; set; }

		string ThemeFontSecondary { get; set; }
	}
}
