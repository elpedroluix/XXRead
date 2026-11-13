using XStory.DTO;
using XStory.DTO.Config;

namespace XXRead.Core.Session
{
	/// <summary>
	/// Registered as singleton.
	/// </summary>
	public class ServiceSession : IServiceSession
	{
		private readonly IServiceSettings _serviceSettings;

		public ServiceSession(IServiceSettings serviceSettings)
		{
			_serviceSettings = serviceSettings;
		}

		public DataSources DataSource
		{
			get => _serviceSettings.DataSource;
			set => _serviceSettings.DataSource = value;
		}

		public Story CurrentStory { get; set; }
		public Author CurrentAuthor { get; set; }
		public Category CurrentCategory { get; set; }

		public List<Story> ListAlreadyLoadedStories { get; } = new List<Story>();
		public List<Author> ListAlreadyLoadedAuthors { get; } = new List<Author>();

		public int PageNumber { get; set; } = 1;
		public int PageNumberAuthor { get; set; } = 1;
	}
}
