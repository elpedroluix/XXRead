using System;
using System.Collections.Generic;
using System.Text;
using XXRead.Core.Models;

namespace XXRead.Core.Services
{
	/// <summary>
	/// Current data source (stored in the user settings, see <see cref="Session.IServiceSettings"/>).
	/// </summary>
	public interface IServiceConfig
	{
		
		/// Gets Stories contexts data sources from a JSON file
		// List<DataSourceItem> GetDataSourcesFromJSON();

		/// <summary>
		/// Sets curret data source from Enum.
		/// </summary>
		/// <returns></returns>
		void SetCurrentDataSource(DataSources dataSource);
		Array GetDataSources();
		DataSources GetCurrentDataSource();
	}
}
