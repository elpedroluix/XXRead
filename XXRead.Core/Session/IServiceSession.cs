using XStory.DTO;
using XStory.DTO.Config;

namespace XXRead.Core.Session
{
	/// <summary>
	/// Navigation state shared by the Core services (replaces BL.Common.StaticContext).
	/// </summary>
	public interface IServiceSession
	{
		/// <summary>
		/// Read from / written to <see cref="IServiceSettings"/> : the only place where the current source is stored.
		/// </summary>
		DataSources DataSource { get; set; }

		Story CurrentStory { get; set; }
		Author CurrentAuthor { get; set; }
		Category CurrentCategory { get; set; }

		List<Story> ListAlreadyLoadedStories { get; }
		List<Author> ListAlreadyLoadedAuthors { get; }

		int PageNumber { get; set; }
		int PageNumberAuthor { get; set; }
	}
}
