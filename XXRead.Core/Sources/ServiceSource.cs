using XXRead.Core.Models;
using XXRead.Core.Session;

namespace XXRead.Core.Sources
{
	public class ServiceSource : IServiceSource
	{
		private readonly IEnumerable<ISource> _sources;
		private readonly IServiceSession _serviceSession;

		public ServiceSource(IEnumerable<ISource> sources, IServiceSession serviceSession)
		{
			_sources = sources;
			_serviceSession = serviceSession;
		}

		public ISource Current => Get(_serviceSession.DataSource);

		public ISource Get(DataSources dataSource)
		{
			List<ISource> sources = _sources.Where(s => s.Id == dataSource).ToList();

			if (sources.Count != 1)
			{
				throw new InvalidOperationException($"{sources.Count} ISource registered for '{dataSource}', 1 expected.");
			}

			return sources[0];
		}
	}
}
