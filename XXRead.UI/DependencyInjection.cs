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

			return services;
		}
	}
}
