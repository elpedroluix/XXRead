using XXRead.Logger;
using XXRead.UI.Services;

namespace XXRead.Tests.UI.Fakes
{
	public class FakeServiceLogs : IServiceLogs
	{
		/// <summary>
		/// Null = error.
		/// </summary>
		public List<Log> Logs { get; set; } = new List<Log>();

		public int GetLogsCount { get; private set; }

		public Task<List<Log>> GetLogs()
		{
			GetLogsCount++;

			return Task.FromResult(Logs);
		}
	}
}
