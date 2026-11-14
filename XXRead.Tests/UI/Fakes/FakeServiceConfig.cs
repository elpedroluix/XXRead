using XXRead.Core.Models;
using XXRead.Core.Services;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// Current data source stored in <see cref="FakeServiceSettings"/>, as the real ServiceConfig does through the session.
	/// </summary>
	public class FakeServiceConfig : IServiceConfig
	{
		private readonly FakeServiceSettings _serviceSettings;

		public FakeServiceConfig(FakeServiceSettings serviceSettings)
		{
			_serviceSettings = serviceSettings;
		}

		public void SetCurrentDataSource(DataSources dataSource) => _serviceSettings.DataSource = dataSource;

		public Array GetDataSources() => Enum.GetValues(typeof(DataSources));

		public DataSources GetCurrentDataSource() => _serviceSettings.DataSource;
	}
}
