using CommunityToolkit.Mvvm.Input;
using XStory.DTO;
using XXRead.Helpers;
using XXRead.Helpers.Services;
using XXRead.Helpers.Themes;

namespace XXRead.ViewModels
{
	public class SettingsPage3ViewModel : BaseViewModel
	{
		#region --- Fields ---

		private XStory.BL.Web.XStory.Contracts.IServiceCategory _serviceCategoryWeb;
		private XStory.BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

		private List<XStory.Logger.Log> _logs;
		public List<XStory.Logger.Log> Logs
		{
			get { return _logs; }
			set { SetProperty(ref _logs, value); }
		}


		private List<Category> _categories;
		public List<Category> Categories
		{
			get { return _categories; }
			set { SetProperty(ref _categories, value); }
		}

		private string _logsPageTitle;
		public string LogsPageTitle
		{
			get { return _logsPageTitle; }
			set { SetProperty(ref _logsPageTitle, value); }
		}

		private FlexLayout _categoriesContentView;
		public FlexLayout CategoriesContentView
		{
			get { return _categoriesContentView; }
			set { SetProperty(ref _categoriesContentView, value); }
		}

		private ContentView _themesContentView;
		public ContentView ThemesContentView
		{
			get { return _themesContentView; }
			set { SetProperty(ref _themesContentView, value); }
		}


		public RelayCommand<object> ThemeBackgroundTappedCommand { get; set; }
		public RelayCommand<object> ThemeMainTappedCommand { get; set; }
		public RelayCommand<string> CategoryTappedCommand { get; set; }
		public RelayCommand DisplayCategoriesViewCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public SettingsPage3ViewModel(INavigationService navigationService,
			XStory.BL.Web.XStory.Contracts.IServiceCategory serviceCategoryWeb,
			XStory.BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite)
			: base(navigationService)
		{
			Title = Helpers.Constants.SettingsPageConstants.SETTINGS_PAGE_TITLE;
			LogsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;

			_serviceCategoryWeb = serviceCategoryWeb;
			_serviceCategorySQLite = serviceCategorySQLite;

			ThemeBackgroundTappedCommand = new RelayCommand<object>((color) => ExecuteThemeBackgroundTappedCommand(color));
			ThemeMainTappedCommand = new RelayCommand<object>((color) => ExecuteThemeMainTappedCommand(color));
			DisplayCategoriesViewCommand = new RelayCommand(ExecuteDisplayCategoriesViewCommand);

			BuildCategories();
			// BuildLogs(); // disabled jusqu a nouvel ordre
		}
		#endregion

		private void ToggleCategoryButtonColor(Button categoryButton)
		{
			if (categoryButton.BackgroundColor == ThemeMain)
			{
				// disabled
				categoryButton.BackgroundColor = Color.FromArgb("#5E5E5E");
				categoryButton.BorderColor = ThemeMain;
				categoryButton.BorderWidth = 2;
				categoryButton.TextColor = Colors.DarkGray;
			}
			else
			{
				// enabled
				categoryButton.BackgroundColor = ThemeMain;
				categoryButton.BorderWidth = 0;
				categoryButton.TextColor = Colors.White;
			}
		}

		private void ExecuteThemeBackgroundTappedCommand(object color)
		{
			if ((Color)color == Color.FromArgb(Theme.DarkPrimary))
			{
				this.ThemePrimary = Color.FromArgb(Theme.DarkPrimary);
				this.ThemeSecondary = Color.FromArgb(Theme.DarkSecondary);

				this.ThemeFontPrimary = Color.FromArgb(Theme.FontLightPrimary);
				this.ThemeFontSecondary = Color.FromArgb(Theme.FontLightSecondary);
			}
			else if ((Color)color == Color.FromArgb(Theme.LightPrimary))
			{
				this.ThemePrimary = Color.FromArgb(Theme.LightPrimary);
				this.ThemeSecondary = Color.FromArgb(Theme.LightSecondary);

				this.ThemeFontPrimary = Color.FromArgb(Theme.FontDarkPrimary);
				this.ThemeFontSecondary = Color.FromArgb(Theme.FontDarkSecondary);
			}

			AppSettings.ThemePrimary = this.ThemePrimary.ToHex();
			AppSettings.ThemeSecondary = this.ThemeSecondary.ToHex();

			AppSettings.ThemeFontPrimary = this.ThemeFontPrimary.ToHex();
			AppSettings.ThemeFontSecondary = this.ThemeFontSecondary.ToHex();

		}

		private void ExecuteThemeMainTappedCommand(object color)
		{
			this.ThemeMain = (Color)color;
			if (this.ThemeMain != Colors.Gold)
			{
				Preferences.Set(nameof(AppSettings.ThemeMain), ThemeMain.ToHex());
				((NavigationPage)Application.Current.MainPage).BarBackgroundColor = this.ThemeMain;
			}
		}

		private async void ExecuteDisplayCategoriesViewCommand()
		{
			var navigationParams = new Dictionary<string, object>()
			{
				{ "categories", Categories }
			};

			await NavigationService.NavigateAsync(nameof(Views.Popup.PopupHiddenCategoriesPage), navigationParams);
		}

		private async void BuildCategories()
		{
			Categories = await this.GetCategories();
		}

		private async void BuildLogs()
		{
			try
			{
				var logs = await XStory.Logger.ServiceLog.GetLogs();
				Logs = logs.OrderByDescending(log => DateTime.Parse(log.Date)).ToList();
			}
			catch (Exception ex)
			{
				XStory.Logger.ServiceLog.Error(ex);
			}
		}

		private async Task<List<Category>> GetCategories()
		{
			// Get categories from database
			// If categories -> get from DB
			// else -> get from web
			List<Category> categories;
			try
			{
				// Categories from SQLite
				categories = await _serviceCategorySQLite.GetCategories(StaticContext.DATASOURCE, true);
				if (categories == null || categories.Count == 0)
				{
					// Categories from web
					categories = await _serviceCategoryWeb.GetCategories();
					if (categories == null || categories.Count == 0)
					{
						throw new Exception("Couldn't get Categories from local DB nor web.");
					}
				}

				return categories.OrderBy(c => c.Title).ToList();

			}
			catch (Exception ex)
			{
				XStory.Logger.ServiceLog.Error(ex);
				categories = null;
			}
			return categories;
		}
	}
}
