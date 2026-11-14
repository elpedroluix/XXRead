using Microsoft.Extensions.DependencyInjection;
using XXRead.Core.Sources;
using XXRead.Web.Demo;
using XXRead.Web.HDS;
using XXRead.Web.XStory;

namespace XXRead.Web
{
	public static class DependencyInjection
	{
		/// <summary>
		/// Registers the <see cref="ISource"/> implementations, each one with its own HttpClient.
		/// </summary>
		public static IServiceCollection AddXXReadWeb(this IServiceCollection services)
		{
			// Named clients : with the default name (the interface name) both sources would share the same configuration
			services.AddHttpClient<ISource, SourceHDS>(SourceHDS.HTTP_CLIENT_NAME, httpClient =>
				{
					httpClient.BaseAddress = new Uri(SourceHDS.BASE_URL);
				})
				.ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler);

			services.AddHttpClient<ISource, SourceXStory>(SourceXStory.HTTP_CLIENT_NAME, httpClient =>
				{
					httpClient.BaseAddress = new Uri(SourceXStory.BASE_URL);
					httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:141.0) Gecko/20100101 Firefox/141.0");
					httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml");
				})
				.ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler);

			services.AddTransient<ISource, SourceDemo>();

			return services;
		}

		private static HttpMessageHandler CreateHttpMessageHandler()
		{
			return new HttpClientHandler()
			{
				// TODO : SSL validation bypass kept from the former RepositoryWebHDS / RepositoryWebXStory, to remove
				ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
				{
					//bypass
					return true;
				},
			};
		}
	}
}
