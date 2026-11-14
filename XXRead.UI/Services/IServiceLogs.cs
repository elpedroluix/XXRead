using XXRead.Logger;

namespace XXRead.UI.Services
{
	/// <summary>
	/// Logs saved by <see cref="ServiceLog"/> (static, SQLite) : behind an interface so that the logs page can be tested with a fake.
	/// </summary>
	public interface IServiceLogs
	{
		/// <summary>
		/// Null on error.
		/// </summary>
		Task<List<Log>> GetLogs();
	}
}
