using XStory.DTO.Config;

namespace XXRead.Core.Session
{
	/// <summary>
	/// Persisted user settings. Implemented by the UI (MAUI Preferences).
	/// </summary>
	public interface IServiceSettings
	{
		DataSources DataSource { get; set; }
	}
}
