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

		private BL.Common.Contracts.IServiceCategory _serviceCategory;
		private BL.Common.Contracts.IServiceConfig _serviceConfig;

		private List<DataSourceItem> _dataSourceItems;

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

		public DelegateCommand StoriesSourceTappedCommand { get; set; }
		public DelegateCommand<object> ThemeBackgroundTappedCommand { get; set; }
		public DelegateCommand<object> ThemeMainTappedCommand { get; set; }
		public DelegateCommand DisplayCategoriesViewCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public SettingsPageViewModel(INavigationService navigationService,
			BL.Common.Contracts.IServiceCategory serviceCategory,
			BL.Common.Contracts.IServiceConfig serviceConfig)
			: base(navigationService)
		{
			Title = Helpers.Constants.SettingsPageConstants.SETTINGS_PAGE_TITLE;
			LogsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;

			_serviceCategory = serviceCategory;
			_serviceConfig = serviceConfig;

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
			await NavigationService.NavigateAsync(nameof(Views.Popup.PopupHiddenCategoriesPage));
		}

		private void BuildDataSourceItems()
		{
			_dataSourceItems = new List<DataSourceItem>();

			foreach (var item in _serviceConfig.GetDataSources())
			{
				_dataSourceItems.Add(new DataSourceItem() { Name = item.ToString(), Image = string.Concat(item.ToString().ToLower(), "_icon") });
			};

			// Get current Datasource
			this.SetVMCurrentDataSource();

		}

		private void SetVMCurrentDataSource()
		{
			CurrentDataSource = _dataSourceItems.First(dsi =>
			dsi.Name.ToLower() == _serviceConfig.GetCurrentDataSource().ToString().ToLower());
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
				if (CurrentDataSource.Name.ToLower() != _serviceConfig.GetCurrentDataSource().ToString().ToLower())
				{
					this.SetVMCurrentDataSource();

					AppSettings.DataSource = CurrentDataSource.Name;
					AppSettings.DataSourceChanged = true;
				}
			}
		}
	}
}
