using System.Text.Json;
using XXRead.Core.Models;
using XXRead.Core.Session;
using XXRead.UI.Platform;
using XXRead.UI.Theming;

namespace XXRead.WebHost.Platform
{
	/// <summary>
	/// Settings of the WebHost, saved in a JSON file of the local application data folder (the MAUI app uses its Preferences).
	/// </summary>
	public class ServiceSettingsJson : IServiceSettings, IServiceAppSettings
	{
		private static readonly string FILE_PATH = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XXRead.WebHost", "settings.json");

		private readonly Lock _lock = new Lock();
		private readonly SettingsValues _values;

		public ServiceSettingsJson()
		{
			_values = Load();
		}

		public DataSources DataSource
		{
			get => _values.DataSource;
			set => Save(() => _values.DataSource = value);
		}

		public bool FirstRun
		{
			get => _values.FirstRun;
			set => Save(() => _values.FirstRun = value);
		}

		public bool DataSourceChanged
		{
			get => _values.DataSourceChanged;
			set => Save(() => _values.DataSourceChanged = value);
		}

		public string ThemeMain
		{
			get => _values.ThemeMain;
			set => Save(() => _values.ThemeMain = value);
		}

		public string ThemePrimary
		{
			get => _values.ThemePrimary;
			set => Save(() => _values.ThemePrimary = value);
		}

		public string ThemeSecondary
		{
			get => _values.ThemeSecondary;
			set => Save(() => _values.ThemeSecondary = value);
		}

		public string ThemeFontPrimary
		{
			get => _values.ThemeFontPrimary;
			set => Save(() => _values.ThemeFontPrimary = value);
		}

		public string ThemeFontSecondary
		{
			get => _values.ThemeFontSecondary;
			set => Save(() => _values.ThemeFontSecondary = value);
		}

		private static SettingsValues Load()
		{
			if (!File.Exists(FILE_PATH))
			{
				return new SettingsValues();
			}

			return JsonSerializer.Deserialize<SettingsValues>(File.ReadAllText(FILE_PATH)) ?? new SettingsValues();
		}

		private void Save(Action change)
		{
			lock (_lock)
			{
				change();

				Directory.CreateDirectory(Path.GetDirectoryName(FILE_PATH));
				File.WriteAllText(FILE_PATH, JsonSerializer.Serialize(_values));
			}
		}

		/// <summary>
		/// Content of the JSON file. Same default values as the MAUI app.
		/// </summary>
		private class SettingsValues
		{
			public DataSources DataSource { get; set; } = DataSources.XStory;
			public bool FirstRun { get; set; } = true;
			public bool DataSourceChanged { get; set; }
			public string ThemeMain { get; set; } = ThemeColors.MainRed;
			public string ThemePrimary { get; set; } = ThemeColors.DarkPrimary;
			public string ThemeSecondary { get; set; } = ThemeColors.DarkSecondary;
			public string ThemeFontPrimary { get; set; } = ThemeColors.FontLightPrimary;
			public string ThemeFontSecondary { get; set; } = ThemeColors.FontLightSecondary;
		}
	}
}
