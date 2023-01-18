using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Xamarin.Essentials;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using XStory.ViewModels;
using XStory.Views;

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

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<StoryPage, StoryPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<StoryInfoPage, StoryInfoViewModel>();

            containerRegistry.RegisterPopupNavigationService();

            containerRegistry.Register<BL.Web.Contracts.IServiceCategory, BL.Web.ServiceCategory>();
            containerRegistry.Register<BL.Web.Contracts.IServiceStory, BL.Web.ServiceStory>();

            containerRegistry.Register<BL.SQLite.Contracts.IServiceCategory, BL.SQLite.ServiceCategory>();
            containerRegistry.Register<BL.SQLite.Contracts.IServiceSettings, BL.SQLite.ServiceSettings>();
        }
    }
}
