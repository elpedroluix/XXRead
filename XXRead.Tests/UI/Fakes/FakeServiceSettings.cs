using XXRead.Core.Models;
using XXRead.Core.Session;

namespace XXRead.Tests.UI.Fakes
{
	public class FakeServiceSettings : IServiceSettings
	{
		public DataSources DataSource { get; set; } = DataSources.Demo;
	}
}
