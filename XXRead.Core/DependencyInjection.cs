using Microsoft.Extensions.DependencyInjection;
using XXRead.Core.Services;
using XXRead.Core.Session;
using XXRead.Core.Sources;

namespace XXRead.Core
{
	public static class DependencyInjection
	{
		/// <summary>
		/// Registers the Core services. The caller must also register <see cref="IServiceSettings"/> (UI),
		/// the <see cref="ISource"/> implementations (XXRead.Web) and the repositories (XXRead.Storage).
		/// </summary>
		public static IServiceCollection AddXXReadCore(this IServiceCollection services)
		{
			services.AddSingleton<IServiceSession, ServiceSession>();

			// Transient : the sources are transient (typed HttpClient), a singleton would keep them forever
			services.AddTransient<IServiceSource, ServiceSource>();

			// Stateless : the navigation state is kept by IServiceSession
			services.AddTransient<IServiceStory, ServiceStory>();
			services.AddTransient<IServiceCategory, ServiceCategory>();
			services.AddTransient<IServiceAuthor, ServiceAuthor>();
			services.AddTransient<IServiceConfig, ServiceConfig>();

			return services;
		}
	}
}
