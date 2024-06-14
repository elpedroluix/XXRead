﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XStory.DTO;
using XXRead.Helpers;
using XXRead.Helpers.Themes;
using XStory.Logger;
using CommunityToolkit.Mvvm.Input;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels
{
	public class SettingsPageViewModel : BaseViewModel
	{
		#region --- Fields ---

		private XStory.BL.Common.Contracts.IServiceCategory _serviceCategory;
		private XStory.BL.Common.Contracts.IServiceConfig _serviceConfig;

		private List<DataSourceItem> _dataSourceItems;

		private List<XStory.Logger.Log> _logs;
		public List<XStory.Logger.Log> Logs
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

		public RelayCommand StoriesSourceTappedCommand { get; set; }
		public RelayCommand<object> ThemeBackgroundTappedCommand { get; set; }
		public RelayCommand<object> ThemeMainTappedCommand { get; set; }
		public RelayCommand DisplayCategoriesViewCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public SettingsPageViewModel(INavigationService navigationService,
			XStory.BL.Common.Contracts.IServiceCategory serviceCategory,
			XStory.BL.Common.Contracts.IServiceConfig serviceConfig)
			: base(navigationService)
		{
			Title = Helpers.Constants.SettingsPageConstants.SETTINGS_PAGE_TITLE;
			LogsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;

			_serviceCategory = serviceCategory;
			_serviceConfig = serviceConfig;

			StoriesSourceTappedCommand = new RelayCommand(ExecuteStoriesSourceTappedCommand);
			ThemeBackgroundTappedCommand = new RelayCommand<object>((color) => ExecuteThemeBackgroundTappedCommand(color));
			ThemeMainTappedCommand = new RelayCommand<object>((color) => ExecuteThemeMainTappedCommand(color));
			DisplayCategoriesViewCommand = new RelayCommand(ExecuteDisplayCategoriesViewCommand);

			BuildDataSourceItems();
			// BuildLogs(); // disabled jusqu a nouvel ordre
		}
		#endregion

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

		private async void ExecuteStoriesSourceTappedCommand()
		{
			var navigationParams = new Dictionary<string, object>()
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
				var logs = await XStory.Logger.ServiceLog.GetLogs();
				Logs = logs.OrderByDescending(log => DateTime.Parse(log.Date)).ToList();
			}
			catch (Exception ex)
			{
				XStory.Logger.ServiceLog.Error(ex);
			}
		}

		/*public override void OnNavigatedTo(INavigationParameters parameters)
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
		}*/
	}
}
