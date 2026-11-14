using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core.Sources;

namespace XXRead.Web.XStory
{
	/// <summary>
	/// xstory-fr.com
	/// </summary>
	public class SourceXStory : ISource
	{
		public const string BASE_URL = @"https://www.xstory-fr.com/";
		public const string HTTP_CLIENT_NAME = "XStory";

		private readonly ScraperStory _scraperStory;
		private readonly ScraperAuthor _scraperAuthor;
		private readonly ScraperCategory _scraperCategory;

		public SourceXStory(HttpClient httpClient)
		{
			RepositoryWeb repositoryWeb = new RepositoryWeb(httpClient);

			_scraperStory = new ScraperStory(repositoryWeb);
			_scraperAuthor = new ScraperAuthor(repositoryWeb, _scraperStory);
			_scraperCategory = new ScraperCategory(repositoryWeb);
		}

		public DataSources Id => DataSources.XStory;

		public Task<List<Story>> GetStoriesPage(int page, string categoryUrl = "", string sortCriterion = "")
		{
			return _scraperStory.GetStoriesPage(page, categoryUrl, sortCriterion);
		}

		public Task<Story> GetStory(string url)
		{
			return _scraperStory.GetStory(url);
		}

		/// <summary>
		/// No author pagination on XStory : <paramref name="pageNumber"/> is ignored.
		/// </summary>
		public Task<Author> GetAuthorPage(Author author, int pageNumber = 1)
		{
			return _scraperAuthor.GetAuthorPage(author);
		}

		public Task<List<Category>> GetCategories()
		{
			return _scraperCategory.GetCategories();
		}

		public Task<List<Tag>> GetTags()
		{
			return Task.FromResult(new List<Tag>());
		}
	}
}
