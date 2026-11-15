using Microsoft.Extensions.Logging;
using XXRead.Core;
using XXRead.Helpers.Services;
using XXRead.Storage;
using XXRead.UI;
using XXRead.UI.Platform;
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
                .RegisterServices()
                .RegisterBlazorUI();

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddBlazorWebViewDeveloperTools();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
        {
            builder.Services.AddXXReadCore().AddXXReadWeb().AddXXReadStorage();
            builder.Services.AddSingleton<XXRead.Core.Session.IServiceSettings, ServiceSettingsPreferences>();

            return builder;
        }

        public static MauiAppBuilder RegisterBlazorUI(this MauiAppBuilder builder)
        {
            builder.Services.AddXXReadUI();
            builder.Services.AddSingleton<IServiceAppSettings, ServiceAppSettingsPreferences>();
            builder.Services.AddSingleton<IServicePlatform, ServicePlatformMAUI>();

            builder.Services.AddTransient<BlazorHostPage>();

            return builder;
        }
    }
}
