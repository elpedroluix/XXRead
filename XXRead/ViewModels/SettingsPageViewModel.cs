using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.Helpers;
using XXRead.Helpers.Themes;
using XXRead.Logger;
using CommunityToolkit.Mvvm.Input;
using XXRead.Helpers.Services;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Behaviors;

namespace XXRead.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        #region --- Fields ---

        private IServicePopup _servicePopup;

        private IServiceCategory _serviceCategory;
        private IServiceConfig _serviceConfig;

        private List<DataSourceItem> _dataSourceItems;

        private List<XXRead.Logger.Log> _logs;
        public List<XXRead.Logger.Log> Logs
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
        #endregion

        #region --- Ctor ---
        public SettingsPageViewModel(IServiceNavigation serviceNavigation,
            IServicePopup servicePopup,
            IServiceCategory serviceCategory,
            IServiceConfig serviceConfig)
            : base(serviceNavigation)
        {
            Title = Helpers.Constants.SettingsPageConstants.SETTINGS_PAGE_TITLE;
            LogsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;

            _servicePopup = servicePopup;

            _serviceCategory = serviceCategory;
            _serviceConfig = serviceConfig;

            StoriesSourceTappedCommand = new RelayCommand(ExecuteStoriesSourceTappedCommand);
            ThemeBackgroundTappedCommand = new RelayCommand<object>((color) => ExecuteThemeBackgroundTappedCommand(color));
            ThemeMainTappedCommand = new RelayCommand<object>((color) => ExecuteThemeMainTappedCommand(color));

            BuildDataSourceItems();
            // BuildLogs(); // disabled jusqu a nouvel ordre
        }
        #endregion

        private void ExecuteThemeBackgroundTappedCommand(object color)
        {
            Color themeBackgroundPrimary;
            Color themeBackgroundSecondary;
            Color themeFontPrimary;
            Color themeFontSecondary;

            if ((color as Color) == null)
            {
                return;
            }

            if (Color.Equals(color, Color.FromArgb(Theme.DarkPrimary)))
            {
                themeBackgroundPrimary = Color.FromArgb(Theme.DarkPrimary);
                themeBackgroundSecondary = Color.FromArgb(Theme.DarkSecondary);

                themeFontPrimary = Color.FromArgb(Theme.FontLightPrimary);
                themeFontSecondary = Color.FromArgb(Theme.FontLightSecondary);
            }
            else // (Color.Equals(color, Color.FromArgb(Theme.LightPrimary)))
            {
                themeBackgroundPrimary = Color.FromArgb(Theme.LightPrimary);
                themeBackgroundSecondary = Color.FromArgb(Theme.LightSecondary);

                themeFontPrimary = Color.FromArgb(Theme.FontDarkPrimary);
                themeFontSecondary = Color.FromArgb(Theme.FontDarkSecondary);

            }

            AppSettings.ThemePrimary = themeBackgroundPrimary.ToHex();
            AppSettings.ThemeSecondary = themeBackgroundSecondary.ToHex();

            AppSettings.ThemeFontPrimary = themeFontPrimary.ToHex();
            AppSettings.ThemeFontSecondary = themeFontSecondary.ToHex();


            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                var themeDictionary = mergedDictionaries.FirstOrDefault(rd => rd.Source.OriginalString.Contains("Theme.xaml"));
                themeDictionary["ThemeBackgroundPrimary"] = themeBackgroundPrimary;
                themeDictionary["ThemeBackgroundSecondary"] = themeBackgroundSecondary;
                themeDictionary["ThemeFontPrimary"] = themeFontPrimary;
                themeDictionary["ThemeFontSecondary"] = themeFontSecondary;
            }

        }

        private void ExecuteThemeMainTappedCommand(object color)
        {
            var themeMain = (Color)color;
            AppSettings.ThemeMain = themeMain.ToHex();

            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                var themeDictionary = mergedDictionaries.FirstOrDefault(rd => rd.Source.OriginalString.Contains("Theme.xaml"));
                themeDictionary["ThemeMain"] = themeMain;
            }

            //((NavigationPage)Application.Current.Windows[0].Page).BarBackgroundColor = themeMain;

            Application.Current.Windows[0].Page.Behaviors.Add(new StatusBarBehavior() { StatusBarStyle = StatusBarStyle.LightContent, StatusBarColor = themeMain, ApplyOn = StatusBarApplyOn.OnBehaviorAttachedTo });
        }

        private async void ExecuteStoriesSourceTappedCommand()
        {
            DataSourceItem currentDS = this.CurrentDataSource;
            var navigationParams = new Dictionary<string, object>()
            {
                { "dataSources", _dataSourceItems }
            };

            var result = await _servicePopup.ShowPopupAsync<Views.Popup.PopupDataSourceSelectionPage, ViewModels.PopupViewModels.PopupDataSourceSelectionPageViewModel, DataSourceItem>(
                onPresenting: viewModel => viewModel.BuildDataSourceItems(_dataSourceItems));
            DataSourceItem? selectedDataSource = result.Result;

            if (selectedDataSource != null && selectedDataSource != currentDS)
            {
                CurrentDataSource = selectedDataSource;

                this.SetCurrentDataSource(selectedDataSource);
            }
        }

        private void SetCurrentDataSource(DataSourceItem dsItem)
        {
            _serviceConfig.SetCurrentDataSource(
                    (XXRead.Core.Models.DataSources)Enum.Parse(typeof(XXRead.Core.Models.DataSources), dsItem.Name));

            _serviceCategory.SetCurrentCategory(null);

            AppSettings.DataSourceChanged = true;
        }

        private void BuildDataSourceItems()
        {
            _dataSourceItems = new List<DataSourceItem>();

            foreach (var item in _serviceConfig.GetDataSources())
            {
                _dataSourceItems.Add(new DataSourceItem() { Name = item.ToString(), Image = string.Concat(item.ToString().ToLower(), "_icon") });
            }
            ;

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
                var logs = await XXRead.Logger.ServiceLog.GetLogs();
                Logs = logs.OrderByDescending(log => DateTime.Parse(log.Date)).ToList();
            }
            catch (Exception ex)
            {
                XXRead.Logger.ServiceLog.Error(ex);
            }
        }
    }
}
