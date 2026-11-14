using XXRead.Core.Models;

namespace XXRead.Core.Sources
{
	/// <summary>
	/// A stories website (HDS, XStory, Demo...).
	/// Implementations throw on error : logging and fallback values are handled by the Core services.
	/// </summary>
	public interface ISource
	{
		DataSources Id { get; }

		Task<List<Story>> GetStoriesPage(int page, string categoryUrl = "", string sortCriterion = "");
		Task<Story> GetStory(string url);

		/// <summary>
		/// Gets the author's infos and stories. Sources without author pagination ignore <paramref name="pageNumber"/>.
		/// </summary>
		Task<Author> GetAuthorPage(Author author, int pageNumber = 1);

		Task<List<Category>> GetCategories();

		/// <summary>
		/// Sources without tags return an empty list.
		/// </summary>
		Task<List<Tag>> GetTags();
	}
}
