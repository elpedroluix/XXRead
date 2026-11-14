using Microsoft.Extensions.DependencyInjection;
using XStory.DTO.Config;
using XXRead.Core;
using XXRead.Core.Session;
using XXRead.Core.Sources;
using XXRead.Web;

namespace XXRead.Tests
{
	/// <summary>
	/// The sources as registered in the app (AddXXReadCore + AddXXReadWeb), calling the real websites.
	/// </summary>
	internal static class SourcesOnline
	{
		private static readonly ServiceProvider _serviceProvider = new ServiceCollection()
			.AddXXReadCore()
			.AddXXReadWeb()
			.AddSingleton<IServiceSettings, ServiceSettingsMemory>()
			.BuildServiceProvider();

		public static IServiceSource ServiceSource => _serviceProvider.GetRequiredService<IServiceSource>();

		public static ISource Get(DataSources dataSource)
		{
			return ServiceSource.Get(dataSource);
		}

		private class ServiceSettingsMemory : IServiceSettings
		{
			public DataSources DataSource { get; set; }
		}
	}
}
