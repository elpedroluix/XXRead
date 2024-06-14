using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXRead.Helpers.Services
{
	public class NavigationService : INavigationService
	{
		public Task GoBackAsync(IDictionary<string, object> routeParameters = null)
		{
			throw new NotImplementedException();
		}

		public Task InitializeAsync()
		{
			throw new NotImplementedException();
		}

		public Task NavigateAsync(string route, IDictionary<string, object> routeParameters = null)
		{
			throw new NotImplementedException();
		}
	}
}
