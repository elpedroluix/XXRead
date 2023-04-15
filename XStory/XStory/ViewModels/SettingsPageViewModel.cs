using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XStory.DTO;
using XStory.Helpers;
using XStory.Helpers.Themes;

namespace XStory.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        #region --- Fields ---

        private BL.Web.Contracts.IServiceCategory _serviceCategoryWeb;
        private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

        private List<Logger.Log> _logs;
        public List<Logger.Log> Logs
        {
            get { return _logs; }
            set { SetProperty(ref _logs, value); }
        }


        private List<DTO.Category> _categories;
        public List<DTO.Category> Categories
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


        public DelegateCommand<object> ThemeBackgroundTappedCommand { get; set; }
        public DelegateCommand<object> ThemeMainTappedCommand { get; set; }
        public DelegateCommand<string> CategoryTappedCommand { get; set; }
        public DelegateCommand DisplayCategoriesViewCommand { get; set; }
        #endregion

        #region --- Ctor ---
        public SettingsPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceCategory serviceCategoryWeb, BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite)
            : base(navigationService)
        {
            Title = Helpers.Constants.SettingsPageConstants.SETTINGS_PAGE_TITLE;
            LogsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;

            _serviceCategoryWeb = serviceCategoryWeb;
            _serviceCategorySQLite = serviceCategorySQLite;

            ThemeBackgroundTappedCommand = new DelegateCommand<object>((color) => ExecuteThemeBackgroundTappedCommand(color));
            ThemeMainTappedCommand = new DelegateCommand<object>((color) => ExecuteThemeMainTappedCommand(color));
            DisplayCategoriesViewCommand = new DelegateCommand(ExecuteDisplayCategoriesViewCommand);

            BuildCategories();
            // BuildLogs(); // disabled jusqu a nouvel ordre
        }
        #endregion

        private void ToggleCategoryButtonColor(Button categoryButton)
        {
            if (categoryButton.BackgroundColor == ThemeMain)
            {
                // disabled
                categoryButton.BackgroundColor = Color.FromHex("#5E5E5E");
                categoryButton.BorderColor = ThemeMain;
                categoryButton.BorderWidth = 2;
                categoryButton.TextColor = Color.DarkGray;
            }
            else
            {
                // enabled
                categoryButton.BackgroundColor = ThemeMain;
                categoryButton.BorderWidth = 0;
                categoryButton.TextColor = Color.White;
            }
        }

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

        private async void ExecuteDisplayCategoriesViewCommand()
        {
            var navigationParams = new NavigationParameters()
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
                var logs = await Logger.ServiceLog.GetLogs();
                Logs = logs.OrderByDescending(log => DateTime.Parse(log.Date)).ToList();
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Error(ex);
            }
        }

        private async Task<List<Category>> GetCategories()
        {
            // Get categories from database
            // If categories -> get from DB
            // else -> get from web
            List<DTO.Category> categories;
            try
            {
                // Categories from SQLite
                categories = await _serviceCategorySQLite.GetCategories(true);
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
                Logger.ServiceLog.Error(ex);
                categories = null;
            }
            return categories;
        }
    }
}
