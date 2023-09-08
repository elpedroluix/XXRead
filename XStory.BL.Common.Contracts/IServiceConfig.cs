using System;
using System.Collections.Generic;
using System.Text;
using XStory.DTO.Config;

namespace XStory.BL.Common.Contracts
{
	/// <summary>
	/// Xamarin.Forms specific configuration (DataSources, Colors, Theming...)
	/// </summary>
	public interface IServiceConfig
	{
		/// <summary>
		/// Gets Stories contexts data sources from a JSON file
		/// </summary>
		/// <returns></returns>
		// List<DataSourceItem> GetDataSourcesFromJSON();

		void SetDataSource(DataSources dataSource);
		Array GetDataSources();
		DataSources GetCurrentDataSource();
	}
}
