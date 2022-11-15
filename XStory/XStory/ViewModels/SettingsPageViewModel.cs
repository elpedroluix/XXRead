using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XStory.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
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


        public DelegateCommand<string> CacaCommand { get; set; }
        public DelegateCommand<string> SelectedCategoryCommand { get; set; }

        public SettingsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Helpers.Constants.SettingsPageConstants.SETTINGS_TITLE;

            CacaCommand = new DelegateCommand<string>((color) => ExecuteCacaCommand(color));
            SelectedCategoryCommand = new DelegateCommand<string>((boolimie) => ExecuteSelectedCategoryCommand(boolimie));

            // BuildCategoriesSettings();
        }

        private void ExecuteSelectedCategoryCommand(string boolimie)
        {
            throw new NotImplementedException();
        }

        private void ExecuteCacaCommand(string color)
        {
            Title = "lolol";

            // CategoriesContentView.Children[0].BackgroundColor = Xamarin.Forms.Color.Brown;
        }

        private void BuildCategoriesSettings()
        {
            CategoriesContentView = new Xamarin.Forms.FlexLayout()
            {
                Direction = Xamarin.Forms.FlexDirection.Row,
                Wrap = Xamarin.Forms.FlexWrap.Wrap,
                Children =
                    {
                        new Xamarin.Forms.Button() { Text = "Gay" },
                        new Xamarin.Forms.Button() { Text = "Hétéro crap 1" },
                        new Xamarin.Forms.Button() { Text = "Hétéro / c nul 2" },
                        new Xamarin.Forms.Button() { Text = "Envie de sucer de la grosse teub" },
                        new Xamarin.Forms.Button() { Text = "J'suis une pute" }
                    }
            };
        }
    }
}
