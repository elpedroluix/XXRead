using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Xamarin.Essentials;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using XStory.ViewModels;
using XStory.ViewModels.Settings;
using XStory.Views;
using XStory.Views.Settings;

namespace XStory
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");

            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex(Helpers.AppSettings.ThemeMain);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();

            /* Pages */
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<StoryPage, StoryPageViewModel>();
            containerRegistry.RegisterForNavigation<StoryInfoPage, StoryInfoPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsAppearancePage, SettingsAppearancePageViewModel>();
            containerRegistry.RegisterForNavigation<WelcomePage, WelcomePageViewModel>();

            /* Popup pages */
            containerRegistry.RegisterForNavigation<Views.Popup.PopupSelectCategoryPage, ViewModels.PopupViewModels.PopupSelectCategoryPageViewModel>();
            containerRegistry.RegisterForNavigation<Views.Popup.PopupHiddenCategoriesPage, ViewModels.PopupViewModels.PopupHiddenCategoriesPageViewModel>();
            containerRegistry.RegisterForNavigation<Views.Popup.PopupChaptersPage, ViewModels.PopupViewModels.PopupChaptersPageViewModel>();

            /* Dependency Injection */
            containerRegistry.Register<BL.Web.Contracts.IServiceCategory, BL.Web.ServiceCategory>();
            containerRegistry.Register<BL.Web.Contracts.IServiceStory, BL.Web.ServiceStory>();

            containerRegistry.Register<BL.SQLite.Contracts.IServiceCategory, BL.SQLite.ServiceCategory>();
            containerRegistry.Register<BL.SQLite.Contracts.IServiceSettings, BL.SQLite.ServiceSettings>();
        }
    }
}
