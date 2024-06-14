using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXRead.Helpers.Services
{
	public interface INavigationService
	{
		Task InitializeAsync();

		Task NavigateAsync(string route, IDictionary<string, object> routeParameters = null);

		Task GoBackAsync(IDictionary<string, object> routeParameters = null);
	}
}
