using Microsoft.AspNetCore.Components.WebView.Maui;
//using CommunityToolkit.Maui;
using XXRead.Data;

namespace XXRead
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				//.UseMauiCommunityToolkit()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();
#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
#endif
			builder.RegisterServices();

			builder.Services.AddSingleton<WeatherForecastService>();

			return builder.Build();
		}

		public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
		{
			mauiAppBuilder.Services.AddSingleton<XStory.DAL.SQLite.Contracts.IXXReadDatabase, XStory.DAL.SQLite.XXReadDatabase>();


			mauiAppBuilder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceAuthor, XStory.BL.Common.ServiceAuthor>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceCategory, XStory.BL.Common.ServiceCategory>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceConfig, XStory.BL.Common.ServiceConfig>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceStory, XStory.BL.Common.ServiceStory>();

			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.DSLocator.Contracts.IServiceAuthor, XStory.BL.Web.DSLocator.ServiceAuthor>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.DSLocator.Contracts.IServiceCategory, XStory.BL.Web.DSLocator.ServiceCategory>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.DSLocator.Contracts.IServiceStory, XStory.BL.Web.DSLocator.ServiceStory>();

			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.XStory.Contracts.IServiceAuthor, XStory.BL.Web.XStory.ServiceAuthor>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.XStory.Contracts.IServiceCategory, XStory.BL.Web.XStory.ServiceCategory>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.XStory.Contracts.IServiceStory, XStory.BL.Web.XStory.ServiceStory>();

			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.HDS.Contracts.IServiceAuthor, XStory.BL.Web.HDS.ServiceAuthor>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.HDS.Contracts.IServiceCategory, XStory.BL.Web.HDS.ServiceCategory>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.HDS.Contracts.IServiceStory, XStory.BL.Web.HDS.ServiceStory>();

			mauiAppBuilder.Services.AddTransient<XStory.BL.Web.Demo.Contracts.IServiceStory, XStory.BL.Web.Demo.ServiceStory>();

			mauiAppBuilder.Services.AddTransient<XStory.BL.SQLite.Contracts.IServiceCategory, XStory.BL.SQLite.ServiceCategory>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.SQLite.Contracts.IServiceSettings, XStory.BL.SQLite.ServiceSettings>();
			mauiAppBuilder.Services.AddTransient<XStory.BL.SQLite.Contracts.IServiceStory, XStory.BL.SQLite.ServiceStory>();



			mauiAppBuilder.Services.AddTransient<XStory.DAL.Web.XStory.Contracts.IRepositoryWebXStory, XStory.DAL.Web.RepositoryWebXStory>();
			mauiAppBuilder.Services.AddTransient<XStory.DAL.Web.HDS.Contracts.IRepositoryWebHDS, XStory.DAL.Web.HDS.RepositoryWebHDS>();

			mauiAppBuilder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryStory, XStory.DAL.SQLite.RepositoryStory>();
			mauiAppBuilder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryCategory, XStory.DAL.SQLite.RepositoryCategory>();
			mauiAppBuilder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryAuthor, XStory.DAL.SQLite.RepositoryAuthor>();
			mauiAppBuilder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryAuthorStory, XStory.DAL.SQLite.RepositoryAuthorStory>();


			return mauiAppBuilder;
		}
	}
}