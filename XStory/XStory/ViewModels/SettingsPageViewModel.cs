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

        private List<DTO.Category> _categories;
        public List<DTO.Category> Categories
        {
            get { return _categories; }
            set { SetProperty(ref _categories, value); }
        }

        private string _settingsPageTitle;
        public string SettingsPageTitle
        {
            get { return _settingsPageTitle; }
            set { SetProperty(ref _settingsPageTitle, value); }
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
            SettingsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_SETTINGS_PAGE_TITLE;
            LogsPageTitle = Helpers.Constants.SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;

            _serviceCategoryWeb = serviceCategoryWeb;
            _serviceCategorySQLite = serviceCategorySQLite;

            ThemeBackgroundTappedCommand = new DelegateCommand<object>((color) => ExecuteThemeBackgroundTappedCommand(color));
            ThemeMainTappedCommand = new DelegateCommand<object>((color) => ExecuteThemeMainTappedCommand(color));
            CategoryTappedCommand = new DelegateCommand<string>((categoryName) => ExecuteCategoryTappedCommand(categoryName));
            DisplayCategoriesViewCommand = new DelegateCommand(ExecuteDisplayCategoriesViewCommand);

            // BuildCategoriesSettings();
            BuildCategories();
            BuildLogs();
        }
        #endregion

        private void ExecuteCategoryTappedCommand(string categoryName)
        {
            Button categoryButton = CategoriesContentView.Children.FirstOrDefault((item) => (item as Button).Text == categoryName) as Button;

            this.ToggleCategoryButtonColor(categoryButton);
        }

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

            await NavigationService.NavigateAsync("PopupCategoryPage", navigationParams);
        }

        private async void BuildCategories()
        {
            Categories = await this.GetCategories();
        }

        private async void BuildCategoriesSettings()
        {
            List<Category> categories = await this.GetCategories();
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    XStory.Logger.ServiceLog.Log("Error", "Couldn't get categories from web : \n" + ex.Message + "\n" + ex.StackTrace, this.GetType().Name, DateTime.Now, Logger.LogType.Error);
            //    return;
            //}

            if (categories == null)
            {
                CategoriesContentView = new FlexLayout();
                Button categoryButton = new Button()
                {
                    Text = Helpers.Constants.SettingsPageConstants.SETTINGS_CATEGORIES_MANUAL,
                    Command = new DelegateCommand(this.BuildCategoriesSettings),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10)
                };
                categoryButton.SetBinding(Button.BackgroundColorProperty, nameof(this.ThemeMain));

                CategoriesContentView.Children.Add(categoryButton);
            }
            else
            {
                CategoriesContentView = new Xamarin.Forms.FlexLayout()
                {
                    Direction = FlexDirection.Row,
                    Wrap = FlexWrap.Wrap,
                    //AlignItems = FlexAlignItems.Stretch,
                };

                foreach (var category in categories)
                {
                    CategoriesContentView.Children.Add(
                        new Button()
                        {
                            Text = category.Title,
                            FontSize = 11,
                            BackgroundColor = ThemeMain,
                            BorderColor = ThemeMain,
                            Command = CategoryTappedCommand,
                            CommandParameter = category.Title,
                        }
                    );
                }
            }
        }

        private async void BuildLogs()
        {

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
                categories = await _serviceCategorySQLite.GetCategories();
                if (categories == null || categories.Count == 0)
                {
                    // Categories from web
                    categories = await _serviceCategoryWeb.GetCategories();
                    if (categories == null || categories.Count == 0)
                    {
                        throw new Exception("Couldn't get Categories from local DB nor web.");
                    }
                }

                return categories;

            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                categories = null;
            }
            return categories;
        }
    }
}
