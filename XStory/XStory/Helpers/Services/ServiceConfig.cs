using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using XStory.Logger;

namespace XStory.Helpers.Services
{
	public class ServiceConfig : IServiceConfig
	{
		private const string JSON_DATASOURCES_FILE = "Helpers.dataSources.json";

		public List<DataSourceItem> GetDataSourcesFromJSON()
		{
			try
			{
				string jsonDataSources = string.Empty;

				// To get dataSources from the JSON file
				var dataSourcesStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
						$"{this.GetType().Assembly.GetName().Name}.{JSON_DATASOURCES_FILE}");
				using (StreamReader sr = new StreamReader(dataSourcesStream))
				{
					jsonDataSources = sr.ReadToEnd();
				};

				List<DataSourceItem> dataSourceItems = System.Text.Json.JsonSerializer.Deserialize<List<DataSourceItem>>(jsonDataSources);

				return dataSourceItems;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return null;
			}
		}
	}
}
