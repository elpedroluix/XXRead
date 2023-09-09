using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XStory.DTO;
using XStory.Helpers;
using XStory.Helpers.Themes;
using XStory.Logger;

namespace XStory.ViewModels
{
	public class SettingsPageViewModel : BaseViewModel
	{
		#region --- Fields ---

		private const string JSON_DATASOURCES_FILE = "Helpers.dataSources.json";

		private BL.Web.DSLocator.Contracts.IServiceCategory _dsServiceCategoryWeb;
		private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

		private BL.Common.Contracts.IServiceCategory _elServiceCategory;
		private BL.Common.Contracts.IServiceConfig _elServiceConfig;

		private List<DataSourceItem> _dataSourceItems;
		private List<DTO.Category> _categories;

		private List<Logger.Log> _logs;
		public List<Logger.Log> Logs
		{
			get { return _logs; }
			set { SetProperty(ref _logs, value); }
		}

		private DataSourceItem _currentDataSource;
		public DataSourceItem CurrentDataSource
		{
			get { return _currentDataSource; }
			set { SetProperty(ref _currentDataSource, value); }
		}

		private string _logsPageTitle;
		public string LogsPageTitle
		{
			get { return _logsPageTitle; }
			set { SetProperty(ref _logsPageTitle, value); }
		}

		private Xamarin.Forms.FlexLayout _categoriesContentView;
		public Xamarin.Forms.FlexLayout CategoriesContentView
		{
			get { return _categoriesContentView; }
			set { SetProperty(ref _categoriesContentView, value); }
		}

		private Xamarin.Forms.ContentView _themesContentView;
		public Xamarin.Forms.ContentView ThemesContentView
		{
			get { return _themesContentView; }
			set { SetProperty(ref _themesContentView, value); }
		}

		public DelegateCommand StoriesSourceTappedCommand { get; set; }
		public DelegateCommand<object> ThemeBackgroundTappedCommand { get; set; }
		public DelegateCommand<object> ThemeMainTappedCommand { get; set; }
		public DelegateCommand<string> CategoryTappedCommand { get; set; }
		public DelegateCommand DisplayCategoriesViewCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public SettingsPageViewModel(INavigationService navigationService,
			BL.Web.DSLocator.Contracts.IServiceCategory dsServiceCategoryWeb,
			BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite,
			BL.Common.Contracts.IServiceCategory elServiceCategory,
			BL.Common.Contracts.IServiceConfig elServiceConfig)
			: base(navigationService)
		{
			Title = Helpers.Constants.SettingsPageConstants.SETTINGS_PAGE_TITLE;
			LogsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;

			_dsServiceCategoryWeb = dsServiceCategoryWeb;
			_serviceCategorySQLite = serviceCategorySQLite;

			_elServiceCategory = elServiceCategory;
			_elServiceConfig = elServiceConfig;

			StoriesSourceTappedCommand = new DelegateCommand(ExecuteStoriesSourceTappedCommand);
			ThemeBackgroundTappedCommand = new DelegateCommand<object>((color) => ExecuteThemeBackgroundTappedCommand(color));
			ThemeMainTappedCommand = new DelegateCommand<object>((color) => ExecuteThemeMainTappedCommand(color));
			DisplayCategoriesViewCommand = new DelegateCommand(ExecuteDisplayCategoriesViewCommand);

			BuildDataSourceItems();
			// BuildLogs(); // disabled jusqu a nouvel ordre
		}
		#endregion

		private void ExecuteThemeBackgroundTappedCommand(object color)
		{
			if ((Color)color == Color.FromHex(Theme.DarkPrimary))
			{
				this.ThemePrimary = Color.FromHex(Theme.DarkPrimary);
				this.ThemeSecondary = Color.FromHex(Theme.DarkSecondary);

				this.ThemeFontPrimary = Color.FromHex(Theme.FontLightPrimary);
				this.ThemeFontSecondary = Color.FromHex(Theme.FontLightSecondary);
			}
			else if ((Color)color == Color.FromHex(Theme.LightPrimary))
			{
				this.ThemePrimary = Color.FromHex(Theme.LightPrimary);
				this.ThemeSecondary = Color.FromHex(Theme.LightSecondary);

				this.ThemeFontPrimary = Color.FromHex(Theme.FontDarkPrimary);
				this.ThemeFontSecondary = Color.FromHex(Theme.FontDarkSecondary);
			}

			AppSettings.ThemePrimary = this.ThemePrimary.ToHex();
			AppSettings.ThemeSecondary = this.ThemeSecondary.ToHex();

			AppSettings.ThemeFontPrimary = this.ThemeFontPrimary.ToHex();
			AppSettings.ThemeFontSecondary = this.ThemeFontSecondary.ToHex();

		}

		private void ExecuteThemeMainTappedCommand(object color)
		{
			this.ThemeMain = (Color)color;
			if (this.ThemeMain != Color.Default)
			{
				Preferences.Set(nameof(AppSettings.ThemeMain), ThemeMain.ToHex());
				((NavigationPage)Application.Current.MainPage).BarBackgroundColor = this.ThemeMain;
			}
		}

		private async void ExecuteStoriesSourceTappedCommand()
		{
			var navigationParams = new NavigationParameters()
			{
				{ "dataSources", _dataSourceItems }
			};

			await NavigationService.NavigateAsync(nameof(Views.Popup.PopupDataSourceSelectionPage), navigationParams);
		}

		private async void ExecuteDisplayCategoriesViewCommand()
		{
			var navigationParams = new NavigationParameters()
			{
				{ "categories", _categories }
			};

			await NavigationService.NavigateAsync(nameof(Views.Popup.PopupHiddenCategoriesPage), navigationParams);
		}

		private void BuildDataSourceItems()
		{
			_dataSourceItems = new List<DataSourceItem>();

			foreach (var item in _elServiceConfig.GetDataSources())
			{
				_dataSourceItems.Add(new DataSourceItem() { Name = item.ToString(), Image = string.Concat(item.ToString().ToLower(), "_icon") });
			};

			// Get current Datasource
			this.SetVMCurrentDataSource();

		}

		private void SetVMCurrentDataSource()
		{
			CurrentDataSource = _dataSourceItems.First(dsi =>
			dsi.Name.ToLower() == _elServiceConfig.GetCurrentDataSource().ToString().ToLower());
		}

		private async void BuildLogs()
		{
			try
			{
				var logs = await Logger.ServiceLog.GetLogs();
				Logs = logs.OrderByDescending(log => DateTime.Parse(log.Date)).ToList();
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
			}
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			if (CurrentDataSource != null)
			{
				if (CurrentDataSource.Name.ToLower() != _elServiceConfig.GetCurrentDataSource().ToString().ToLower())
				{
					this.SetVMCurrentDataSource();

					AppSettings.DataSource = CurrentDataSource.Name;
					AppSettings.DataSourceChanged = true;
				}
			}
		}
	}
}
