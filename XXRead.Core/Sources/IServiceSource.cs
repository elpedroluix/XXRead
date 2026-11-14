using XXRead.Core.Models;

namespace XXRead.Core.Sources
{
	public interface IServiceSource
	{
		/// <summary>
		/// Source currently selected by the user (<see cref="Session.IServiceSession.DataSource"/>).
		/// </summary>
		ISource Current { get; }

		ISource Get(DataSources dataSource);
	}
}
