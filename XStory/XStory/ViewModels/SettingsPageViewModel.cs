using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XStory.DTO;

namespace XStory.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private BL.Web.Contracts.IServiceCategory _serviceCategoryWeb;
        private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

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


        public DelegateCommand<string> ThemeTappedCommand { get; set; }
        public DelegateCommand<string> CategoryTappedCommand { get; set; }

        public SettingsPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceCategory serviceCategoryWeb)
            : base(navigationService)
        {
            Title = Helpers.Constants.SettingsPageConstants.SETTINGS_TITLE;

            _serviceCategoryWeb = serviceCategoryWeb;

            ThemeTappedCommand = new DelegateCommand<string>((color) => ExecuteThemeTappedCommand(color));
            CategoryTappedCommand = new DelegateCommand<string>((boolimie) => ExecuteCategoryTappedCommand(boolimie));

            BuildCategoriesSettings();
        }

        private void ExecuteCategoryTappedCommand(string boolimie)
        {
            Button categoryButton = CategoriesContentView.Children.FirstOrDefault((item) => (item as Button).Text == boolimie) as Button;

            this.ToggleCategoryButtonColor(categoryButton);
        }

        private void ToggleCategoryButtonColor(Button categoryButton)
        {
            if (categoryButton.BackgroundColor == Color.Green)
            {
                categoryButton.BackgroundColor = Color.Red;
            }
            else
            {
                categoryButton.BackgroundColor = Color.Green;
            }
        }

        private void ExecuteThemeTappedCommand(string color)
        {
            Title = "lolol";

            // CategoriesContentView.Children[0].BackgroundColor = Xamarin.Forms.Color.Brown;
        }

        private async void BuildCategoriesSettings()
        {
            List<Category> categories;
            try
            {
                categories = await this.GetCategories();
            }
            catch (Exception)
            {
                return;
            }

            CategoriesContentView = new Xamarin.Forms.FlexLayout()
            {
                Direction = Xamarin.Forms.FlexDirection.Row,
                Wrap = Xamarin.Forms.FlexWrap.Wrap
            };

            foreach (var category in categories)
            {
                CategoriesContentView.Children.Add(
                    new Button()
                    {
                        Text = category.Title,
                        FontSize = 11,
                        BackgroundColor = Color.Green,
                        Command = ThemeTappedCommand,
                        CommandParameter = category.Title
                    }
                );
            }
        }

        private async Task<List<Category>> GetCategories()
        {
            // Get categories from database
            // If categories -> get from DB
            // else -> get from web
            try
            {

            }
            catch (Exception)
            {

                throw;
            }

            return await _serviceCategoryWeb.GetCategories();
        }
    }
}
