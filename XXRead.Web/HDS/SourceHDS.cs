using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core.Sources;

namespace XXRead.Web.HDS
{
	/// <summary>
	/// histoires-de-sexe.net
	/// </summary>
	public class SourceHDS : ISource
	{
		public const string BASE_URL = @"https://www.histoires-de-sexe.net/";
		public const string HTTP_CLIENT_NAME = "HDS";

		private readonly ScraperStory _scraperStory;
		private readonly ScraperAuthor _scraperAuthor;
		private readonly ScraperCategory _scraperCategory;
		private readonly ScraperTags _scraperTags;

		public SourceHDS(HttpClient httpClient)
		{
			RepositoryWeb repositoryWeb = new RepositoryWeb(httpClient);

			_scraperStory = new ScraperStory(repositoryWeb);
			_scraperAuthor = new ScraperAuthor(repositoryWeb, _scraperStory);
			_scraperCategory = new ScraperCategory(repositoryWeb);
			_scraperTags = new ScraperTags(repositoryWeb);
		}

		public DataSources Id => DataSources.HDS;

		public Task<List<Story>> GetStoriesPage(int page, string categoryUrl = "", string sortCriterion = "")
		{
			return _scraperStory.GetStoriesPage(page, categoryUrl, sortCriterion);
		}

		public Task<Story> GetStory(string url)
		{
			return _scraperStory.GetStory(url);
		}

		public Task<Author> GetAuthorPage(Author author, int pageNumber = 1)
		{
			return _scraperAuthor.GetAuthorPage(author, pageNumber);
		}

		public Task<List<Category>> GetCategories()
		{
			return _scraperCategory.GetCategories();
		}

		public Task<List<Tag>> GetTags()
		{
			return _scraperTags.GetTags();
		}
	}
}
