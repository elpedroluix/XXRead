using XXRead.Logger;

namespace XXRead.UI.Services
{
	public class ServiceLogs : IServiceLogs
	{
		public Task<List<Log>> GetLogs()
		{
			return ServiceLog.GetLogs();
		}
	}
}
