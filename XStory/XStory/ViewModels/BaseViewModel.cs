using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using XStory.Helpers.Themes;

namespace XStory.ViewModels
{
    public class BaseViewModel : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }

        public DelegateCommand AppearingCommand { get; set; }

        private string _title;
        private Color _themeMain;
        private Color _themePrimary;
        private Color _themeSecondary;
        private Color _themeFontPrimary;
        private Color _themeFontSecondary;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public Color ThemePrimary
        {
            get { return _themePrimary; }
            set { SetProperty(ref _themePrimary, value); }
        }

        public Color ThemeSecondary
        {
            get { return _themeSecondary; }
            set { SetProperty(ref _themeSecondary, value); }
        }

        public Color ThemeMain
        {
            get { return _themeMain; }
            set { SetProperty(ref _themeMain, value); }
        }

        public Color ThemeFontPrimary
        {
            get { return _themeFontPrimary; }
            set { SetProperty(ref _themeFontPrimary, value); }
        }

        public Color ThemeFontSecondary
        {
            get { return _themeFontSecondary; }
            set { SetProperty(ref _themeFontSecondary, value); }
        }

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;

            InitTheming();
        }

        protected void InitTheming()
        {
            ThemeMain = Color.FromHex(Helpers.AppSettings.ThemeMain);
            ThemePrimary = Color.FromHex(Helpers.AppSettings.ThemePrimary);
            ThemeSecondary = Color.FromHex(Helpers.AppSettings.ThemeSecondary);
            ThemeFontPrimary = Color.FromHex(Helpers.AppSettings.ThemeFontPrimary);
            ThemeFontSecondary = Color.FromHex(Helpers.AppSettings.ThemeFontSecondary);
        }

        protected virtual void ExecuteAppearingCommand()
        {

        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
