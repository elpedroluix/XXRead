using CommunityToolkit.Mvvm.Input;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.UI.Constants;
using XXRead.UI.Dialogs;
using XXRead.UI.Platform;
using XXRead.UI.Services;
using XXRead.UI.Theming;

namespace XXRead.UI.ViewModels
{
	/// <summary>
	/// Copied from the XAML UI's SettingsPageViewModel. The colors are applied by <see cref="IServiceTheme"/>
	/// instead of the application resources, the main color comes from the XAML UI's SettingsPageThemesView.
	/// </summary>
	public class SettingsPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private IServiceConfig _serviceConfig;
		private IServiceCategory _serviceCategory;
		private IServiceDialog _serviceDialog;
		private IServiceAppSettings _serviceAppSettings;
		private IServiceTheme _serviceTheme;

		private DataSources _currentDataSource;
		public DataSources CurrentDataSource
		{
			get { return _currentDataSource; }
			set { SetProperty(ref _currentDataSource, value); }
		}

		private ThemeBackground _currentBackground;
		/// <summary>
		/// Null when the stored colors match no predefined background.
		/// </summary>
		public ThemeBackground CurrentBackground
		{
			get { return _currentBackground; }
			set { SetProperty(ref _currentBackground, value); }
		}

		private string _currentMainColor;
		public string CurrentMainColor
		{
			get { return _currentMainColor; }
			set { SetProperty(ref _currentMainColor, value); }
		}
		#endregion

		#region --- Commands ---
		public RelayCommand StoriesSourceTappedCommand { get; set; }
		public RelayCommand<ThemeBackground> ThemeBackgroundTappedCommand { get; set; }
		public RelayCommand<string> ThemeMainTappedCommand { get; set; }
		public RelayCommand LogsTappedCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public SettingsPageViewModel(IServiceNavigation serviceNavigation,
			IServiceDialog serviceDialog,
			IServiceAppSettings serviceAppSettings,
			IServiceTheme serviceTheme,
			IServiceConfig serviceConfig,
			IServiceCategory serviceCategory) : base(serviceNavigation)
		{
			_serviceDialog = serviceDialog;
			_serviceAppSettings = serviceAppSettings;
			_serviceTheme = serviceTheme;
			_serviceConfig = serviceConfig;
			_serviceCategory = serviceCategory;

			Title = SettingsPageConstants.SETTINGS_PAGE_TITLE;

			StoriesSourceTappedCommand = new RelayCommand(ExecuteStoriesSourceTappedCommand);
			ThemeBackgroundTappedCommand = new RelayCommand<ThemeBackground>(ExecuteThemeBackgroundTappedCommand);
			ThemeMainTappedCommand = new RelayCommand<string>(ExecuteThemeMainTappedCommand);
			LogsTappedCommand = new RelayCommand(ExecuteLogsTappedCommand);

			CurrentDataSource = _serviceConfig.GetCurrentDataSource();
			CurrentBackground = ThemeColors.Backgrounds.FirstOrDefault(background => IsSameColor(background.Primary, _serviceAppSettings.ThemePrimary));
			CurrentMainColor = ThemeColors.MainColors.FirstOrDefault(color => IsSameColor(color, _serviceAppSettings.ThemeMain));
		}
		#endregion

		/// <summary>
		/// Data source selection dialog. The stories are reloaded when the user goes back to the MainPage (DataSourceChanged).
		/// </summary>
		private async void ExecuteStoriesSourceTappedCommand()
		{
			DataSources? selectedDataSource = await _serviceDialog.ShowAsync<DialogDataSourceSelection, DataSources?>();
			if (selectedDataSource == null || selectedDataSource == CurrentDataSource)
			{
				return;
			}

			_serviceConfig.SetCurrentDataSource(selectedDataSource.Value);
			_serviceCategory.SetCurrentCategory(null);
			_serviceAppSettings.DataSourceChanged = true;

			CurrentDataSource = selectedDataSource.Value;
		}

		private void ExecuteThemeBackgroundTappedCommand(ThemeBackground background)
		{
			if (background == null)
			{
				return;
			}

			_serviceTheme.SetBackground(background);
			CurrentBackground = background;
		}

		private void ExecuteThemeMainTappedCommand(string color)
		{
			if (string.IsNullOrEmpty(color))
			{
				return;
			}

			_serviceTheme.SetMainColor(color);
			CurrentMainColor = color;
		}

		private async void ExecuteLogsTappedCommand()
		{
			await ServiceNavigation.NavigateTo<Pages.LogsPage>();
		}

		/// <summary>
		/// The MAUI host gives uppercase colors, the WebHost gives them as stored.
		/// </summary>
		private static bool IsSameColor(string color, string otherColor)
		{
			return string.Equals(color, otherColor, StringComparison.OrdinalIgnoreCase);
		}
	}
}
