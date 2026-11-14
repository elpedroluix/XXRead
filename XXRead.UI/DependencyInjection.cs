using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using XXRead.UI.Services;

namespace XXRead.UI
{
	public static class DependencyInjection
	{
		/// <summary>
		/// Registers MudBlazor and the UI services. The host must also register
		/// <see cref="Platform.IServiceAppSettings"/>, <see cref="Platform.IServicePlatform"/>
		/// and <see cref="XXRead.Core.Session.IServiceSettings"/>.
		/// </summary>
		public static IServiceCollection AddXXReadUI(this IServiceCollection services)
		{
			services.AddMudServices();

			// Scoped : NavigationManager, IJSRuntime and IDialogService are scoped (one scope per WebView or per circuit)
			services.AddScoped<IServiceNavigation, ServiceNavigation>();
			services.AddScoped<IServiceDialog, ServiceDialog>();

			services.AddSingleton<IServiceTheme, ServiceTheme>();

			services.RegisterViewModels();

			return services;
		}

		/// <summary>
		/// Scoped : a page gets the same ViewModel back when the user returns to it (Blazor creates a new component on each navigation).
		/// Transient : the page starts again from the session each time (current story).
		/// </summary>
		private static IServiceCollection RegisterViewModels(this IServiceCollection services)
		{
			services.AddScoped<ViewModels.MainPageViewModel>();
			services.AddTransient<ViewModels.StoryPageViewModel>();

			return services;
		}
	}
}
