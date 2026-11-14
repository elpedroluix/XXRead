using XXRead.Core.Models;
using XXRead.Core.Session;

namespace XXRead.Helpers.Services
{
	/// <summary>
	/// <see cref="IServiceSettings"/> stored in the MAUI Preferences (<see cref="AppSettings"/>).
	/// </summary>
	public class ServiceSettingsPreferences : IServiceSettings
	{
		public DataSources DataSource
		{
			get => AppSettings.DataSource;
			set => AppSettings.DataSource = value;
		}
	}
}
