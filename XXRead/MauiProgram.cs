using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using XXRead.Core;
using XXRead.Helpers;
using XXRead.Helpers.Services;
using XXRead.Storage;
using XXRead.Web;

namespace XXRead
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options =>
                {
                    // Popup v2 adds padding, a rounded border and a shadow by default : the popups draw their own frame
                    options.SetPopupDefaults(new DefaultPopupSettings
                    {
                        BackgroundColor = Colors.Transparent,
                        Padding = 0,
                        Margin = 0
                    });
                    options.SetPopupOptionsDefaults(new DefaultPopupOptionsSettings
                    {
                        Shape = null,
                        Shadow = null
                    });
                })
                .RegisterViews()
                .RegisterViewModels()
                .RegisterPopups()
                .RegisterWebServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                //.ConfigureMauiHandlers(handlers =>
                //{
                //	handlers.AddHandler(typeof(Helpers.CustomControls.LabelJustify), typeof(Helpers.Handlers.LabelJustifyHandler));
                //})
                ;

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazorWebViewDeveloperTools();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            /* UI Services*/
            builder.Services.AddSingleton<XXRead.Core.Session.IServiceSettings, ServiceSettingsPreferences>();

            return builder.Build();
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IServiceNavigation, ServiceNavigation>();

            builder.Services.AddTransient<Views.AuthorPage>();
            builder.Services.AddTransient<Views.MainPage>();
            builder.Services.AddTransient<Views.SettingsPage>();
            builder.Services.AddTransient<Views.Settings.SettingsAppearancePage>();
            builder.Services.AddTransient<Views.StoryInfoPage>();
            builder.Services.AddTransient<Views.StoryPage>();
            builder.Services.AddTransient<Views.WelcomePage>();
            return builder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<ViewModels.AuthorPageViewModel>();
            builder.Services.AddTransient<ViewModels.AuthorPageWebViewModel>();
            builder.Services.AddTransient<ViewModels.MainPageViewModel>();
            builder.Services.AddTransient<ViewModels.StoryInfoPageViewModel>();
            builder.Services.AddTransient<ViewModels.StoryPageViewModel>();
            builder.Services.AddTransient<ViewModels.SettingsPageViewModel>();
            builder.Services.AddTransient<ViewModels.Settings.SettingsAppearancePageViewModel>();
            builder.Services.AddTransient<ViewModels.WelcomePageViewModel>();
            return builder;
        }

        public static MauiAppBuilder RegisterPopups(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<IServicePopup, ServicePopup>();
            builder.Services.AddTransient<ViewModels.PopupViewModels.BasePopupViewModel>();

            builder.Services.AddTransientPopup<Views.Popup.PopupChaptersPage, ViewModels.PopupViewModels.PopupChaptersPageViewModel>();
            builder.Services.AddTransientPopup<Views.Popup.PopupDataSourceSelectionPage, ViewModels.PopupViewModels.PopupDataSourceSelectionPageViewModel>();
            builder.Services.AddTransientPopup<Views.Popup.PopupFlyoutMenuPage, ViewModels.PopupViewModels.PopupFlyoutMenuPageViewModel>();
            builder.Services.AddTransientPopup<Views.Popup.PopupSelectCategoryPage, ViewModels.PopupViewModels.PopupSelectCategoryPageViewModel>();
            builder.Services.AddTransientPopup<Views.Popup.PopupStoryActionsPage, ViewModels.PopupViewModels.PopupStoryActionsPageViewModel>();

            return builder;
        }

        public static MauiAppBuilder RegisterWebServices(this MauiAppBuilder builder)
        {
            /* --- Core / Web / Storage --- */
            builder.Services.AddXXReadCore().AddXXReadWeb().AddXXReadStorage();

            return builder;
        }
    }
}
