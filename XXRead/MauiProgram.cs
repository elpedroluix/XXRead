﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using XXRead.Helpers.Services;

namespace XXRead
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit()
				.RegisterViews()
				.RegisterViewModels()
				.RegisterPopupViews()
				.RegisterPopupViewModels()
				.RegisterWebServices()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			/* UI Services*/
			builder.Services.AddSingleton<INavigationService, NavigationService>();

			/* Database */
			builder.Services.AddSingleton<XStory.DAL.SQLite.Contracts.IXXReadDatabase, XStory.DAL.SQLite.XXReadDatabase>();

			return builder.Build();
		}



		public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
		{
			builder.Services.AddTransient<Views.AuthorPage>();
			builder.Services.AddTransient<Views.MainPage>();
			builder.Services.AddTransient<Views.SettingsPage>();
			builder.Services.AddTransient<Views.SettingsPage2>();
			builder.Services.AddTransient<Views.StoryInfoPage>();
			builder.Services.AddTransient<Views.StoryPage>();
			builder.Services.AddTransient<Views.WelcomePage>();

			//Register all routes for Shell
			Routing.RegisterRoute(nameof(Views.AuthorPage), typeof(Views.AuthorPage));
			Routing.RegisterRoute(nameof(Views.MainPage), typeof(Views.MainPage));
			Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
			Routing.RegisterRoute(nameof(Views.StoryInfoPage), typeof(Views.StoryInfoPage));
			Routing.RegisterRoute(nameof(Views.StoryPage), typeof(Views.StoryPage));
			Routing.RegisterRoute(nameof(Views.WelcomePage), typeof(Views.WelcomePage));
			return builder;
		}

		public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
		{
			builder.Services.AddTransient<ViewModels.AuthorPageViewModel>();
			builder.Services.AddTransient<ViewModels.MainPageViewModel>();
			builder.Services.AddTransient<ViewModels.StoryInfoPageViewModel>();
			builder.Services.AddTransient<ViewModels.StoryPageViewModel>();
			builder.Services.AddTransient<ViewModels.SettingsPageViewModel>();
			builder.Services.AddTransient<ViewModels.Settings.SettingsAppearancePageViewModel>();
			builder.Services.AddTransient<ViewModels.WelcomePageViewModel>();
			return builder;
		}

		public static MauiAppBuilder RegisterPopupViews(this MauiAppBuilder builder)
		{
			/* Views */
			builder.Services.AddTransient<Views.Popup.PopupChaptersPage>();
			builder.Services.AddTransient<Views.Popup.PopupDataSourceSelectionPage>();
			builder.Services.AddTransient<Views.Popup.PopupFlyoutMenuPage>();
			builder.Services.AddTransient<Views.Popup.PopupHiddenCategoriesPage>();
			builder.Services.AddTransient<Views.Popup.PopupSelectCategoryPage>();
			builder.Services.AddTransient<Views.Popup.PopupStoryActionsPage>();

			//Register all routes for Shell
			Routing.RegisterRoute(nameof(Views.Popup.PopupChaptersPage), typeof(Views.Popup.PopupChaptersPage));
			Routing.RegisterRoute(nameof(Views.Popup.PopupDataSourceSelectionPage), typeof(Views.Popup.PopupDataSourceSelectionPage));
			Routing.RegisterRoute(nameof(Views.Popup.PopupFlyoutMenuPage), typeof(Views.Popup.PopupFlyoutMenuPage));
			Routing.RegisterRoute(nameof(Views.Popup.PopupHiddenCategoriesPage), typeof(Views.Popup.PopupHiddenCategoriesPage));
			Routing.RegisterRoute(nameof(Views.Popup.PopupSelectCategoryPage), typeof(Views.Popup.PopupSelectCategoryPage));
			Routing.RegisterRoute(nameof(Views.Popup.PopupStoryActionsPage), typeof(Views.Popup.PopupStoryActionsPage));
			return builder;
		}

		public static MauiAppBuilder RegisterPopupViewModels(this MauiAppBuilder builder)
		{
			//builder.Services.AddTransientPopup<ViewModels.PopupViewModels.PopupChaptersPageViewModel>();
			builder.Services.AddTransient<ViewModels.PopupViewModels.BasePopupViewModel>();
			builder.Services.AddTransient<ViewModels.PopupViewModels.PopupChaptersPageViewModel>();
			builder.Services.AddTransient<ViewModels.PopupViewModels.PopupDataSourceSelectionPageViewModel>();
			builder.Services.AddTransient<ViewModels.PopupViewModels.PopupFlyoutMenuPageViewModel>();
			builder.Services.AddTransient<ViewModels.PopupViewModels.PopupHiddenCategoriesPageViewModel>();
			builder.Services.AddTransient<ViewModels.PopupViewModels.PopupSelectCategoryPageViewModel>();
			builder.Services.AddTransient<ViewModels.PopupViewModels.PopupStoryActionsPageViewModel>();
			return builder;
		}

		public static MauiAppBuilder RegisterWebServices(this MauiAppBuilder builder)
		{
			/* --- BL --- */
			builder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceStory, XStory.BL.Common.ServiceStory>();
			builder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceCategory, XStory.BL.Common.ServiceCategory>();
			builder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceAuthor, XStory.BL.Common.ServiceAuthor>();
			builder.Services.AddTransient<XStory.BL.Common.Contracts.IServiceConfig, XStory.BL.Common.ServiceConfig>();

			builder.Services.AddTransient<XStory.BL.Web.DSLocator.Contracts.IServiceStory, XStory.BL.Web.DSLocator.ServiceStory>();
			builder.Services.AddTransient<XStory.BL.Web.DSLocator.Contracts.IServiceCategory, XStory.BL.Web.DSLocator.ServiceCategory>();
			builder.Services.AddTransient<XStory.BL.Web.DSLocator.Contracts.IServiceAuthor, XStory.BL.Web.DSLocator.ServiceAuthor>();

			builder.Services.AddTransient<XStory.BL.Web.XStory.Contracts.IServiceCategory, XStory.BL.Web.XStory.ServiceCategory>();
			builder.Services.AddTransient<XStory.BL.Web.XStory.Contracts.IServiceStory, XStory.BL.Web.XStory.ServiceStory>();
			builder.Services.AddTransient<XStory.BL.Web.XStory.Contracts.IServiceAuthor, XStory.BL.Web.XStory.ServiceAuthor>();

			builder.Services.AddTransient<XStory.BL.Web.HDS.Contracts.IServiceCategory, XStory.BL.Web.HDS.ServiceCategory>();
			builder.Services.AddTransient<XStory.BL.Web.HDS.Contracts.IServiceStory, XStory.BL.Web.HDS.ServiceStory>();
			builder.Services.AddTransient<XStory.BL.Web.HDS.Contracts.IServiceAuthor, XStory.BL.Web.HDS.ServiceAuthor>();

			builder.Services.AddTransient<XStory.BL.Web.Demo.Contracts.IServiceStory, XStory.BL.Web.Demo.ServiceStory>();
			builder.Services.AddTransient<XStory.BL.Web.Demo.Contracts.IServiceAuthor, XStory.BL.Web.Demo.ServiceAuthor>();

			builder.Services.AddTransient<XStory.BL.SQLite.Contracts.IServiceCategory, XStory.BL.SQLite.ServiceCategory>();
			builder.Services.AddTransient<XStory.BL.SQLite.Contracts.IServiceSettings, XStory.BL.SQLite.ServiceSettings>();
			builder.Services.AddTransient<XStory.BL.SQLite.Contracts.IServiceStory, XStory.BL.SQLite.ServiceStory>();
			builder.Services.AddTransient<XStory.BL.SQLite.Contracts.IServiceAuthor, XStory.BL.SQLite.ServiceAuthor>();


			/* --- DAL --- */

			// builder.Services.AddTransient<XStory.DAL.Web.HDS.Contracts.IRepositoryWebHDS, XStory.DAL.Web.HDS.RepositoryWebHDS>();
			// ↑↑↑ mis en commentaire pour tester si utile (car RepositoryXStory manquant, et pourtant ça build) ↑↑↑

			builder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryStory, XStory.DAL.SQLite.RepositoryStory>();
			builder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryCategory, XStory.DAL.SQLite.RepositoryCategory>();
			builder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryAuthor, XStory.DAL.SQLite.RepositoryAuthor>();
			builder.Services.AddTransient<XStory.DAL.SQLite.Contracts.IRepositoryAuthorStory, XStory.DAL.SQLite.RepositoryAuthorStory>();

			return builder;
		}
	}
}
