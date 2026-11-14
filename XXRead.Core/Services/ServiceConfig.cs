using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using XXRead.Core.Models;
using XXRead.Core.Session;

namespace XXRead.Core.Services
{
	public class ServiceConfig : IServiceConfig
	{
		private IServiceSession _serviceSession;

		public ServiceConfig(IServiceSession serviceSession)
		{
			_serviceSession = serviceSession;
		}

		public void SetCurrentDataSource(DataSources dataSource)
		{
			_serviceSession.DataSource = dataSource;
		}

		public Array GetDataSources()
		{
			return Enum.GetValues(typeof(DataSources));
		}

		public DataSources GetCurrentDataSource()
		{
			return _serviceSession.DataSource;
		}
	}
}
