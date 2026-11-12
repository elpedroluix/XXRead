using XStory.BL.Web.HDS;
using XStory.BL.Web.HDS.Contracts;
using XStory.DAL.Web.HDS;
using XStory.DTO;

namespace XXRead.Tests.GoldenMaster
{
	/// <summary>
	/// HDS scraping on saved HTML pages, compared to the reference JSON (see <see cref="GoldenMaster"/>).
	/// </summary>
	[TestClass]
	public class GoldenMasterHDSTests
	{
		private const string SOURCE = "HDS";

		private const string CATEGORY_URL = "/sexe/histoires-cat-hf.php";
		private const string STORY_URL_1 = "https://www.histoires-de-sexe.net/les-ex-de-la-meme-fille-%289%29-54893";
		private const string STORY_URL_2 = "https://www.histoires-de-sexe.net/baiser-a-un-anniversaire-57989";

		// Author with many pages (64 when the fixtures were recorded)
		private const string AUTHOR_NAME = "Nico T";
		private const string AUTHOR_URL = "/fiche.php?auteur=Nico T";
		private const int AUTHOR_LAST_PAGE = 64;

		private HttpHandlerFixtures _handler;
		private HttpClient _originalHttpClient;

		[TestInitialize]
		public void Init()
		{
			_handler = new HttpHandlerFixtures(GoldenMaster.GetFixturesDirectory(SOURCE));
			_originalHttpClient = GoldenMaster.SwapHttpClient(typeof(RepositoryWebHDS), new RepositoryWebHDS().GetHttpClient(), _handler);
		}

		[TestCleanup]
		public void Cleanup()
		{
			GoldenMaster.RestoreHttpClient(typeof(RepositoryWebHDS), _originalHttpClient);
		}

		[TestMethod]
		public async Task StoriesPage1()
		{
			List<Story> result = await new ServiceStory().GetStoriesPage();

			GoldenMaster.Check(SOURCE, nameof(StoriesPage1), result, _handler);
		}

		[TestMethod]
		public async Task StoriesPage2()
		{
			List<Story> result = await new ServiceStory().GetStoriesPage(2);

			GoldenMaster.Check(SOURCE, nameof(StoriesPage2), result, _handler);
		}

		[TestMethod]
		public async Task StoriesCategoryPage1()
		{
			List<Story> result = await new ServiceStory().GetStoriesPage(0, CATEGORY_URL);

			GoldenMaster.Check(SOURCE, nameof(StoriesCategoryPage1), result, _handler);
		}

		[TestMethod]
		public async Task StoriesCategoryPage2()
		{
			List<Story> result = await new ServiceStory().GetStoriesPage(2, CATEGORY_URL);

			GoldenMaster.Check(SOURCE, nameof(StoriesCategoryPage2), result, _handler);
		}

		[TestMethod]
		public async Task Story1()
		{
			Story result = await new ServiceStory().GetStory(STORY_URL_1);

			GoldenMaster.Check(SOURCE, nameof(Story1), result, _handler);
		}

		[TestMethod]
		public async Task Story2()
		{
			Story result = await new ServiceStory().GetStory(STORY_URL_2);

			GoldenMaster.Check(SOURCE, nameof(Story2), result, _handler);
		}

		[TestMethod]
		public async Task AuthorPage1()
		{
			Author result = await NewServiceAuthor().GetAuthorPage(NewAuthor());

			GoldenMaster.Check(SOURCE, nameof(AuthorPage1), result, _handler);
		}

		/// <summary>
		/// Same as the app: page 2 is loaded on the author of page 1 (the stories are added).
		/// </summary>
		[TestMethod]
		public async Task AuthorPage2()
		{
			IServiceAuthor serviceAuthor = NewServiceAuthor();
			Author author = NewAuthor();

			await serviceAuthor.GetAuthorPage(author);
			Author result = await serviceAuthor.GetAuthorPage(author, 2);

			GoldenMaster.Check(SOURCE, nameof(AuthorPage2), result, _handler);
		}

		[TestMethod]
		public async Task AuthorLastPage()
		{
			Author result = await NewServiceAuthor().GetAuthorPage(NewAuthor(), AUTHOR_LAST_PAGE);

			GoldenMaster.Check(SOURCE, nameof(AuthorLastPage), result, _handler);
		}

		[TestMethod]
		public async Task Categories()
		{
			List<Category> result = await new ServiceCategory().GetCategories();

			GoldenMaster.Check(SOURCE, nameof(Categories), result, _handler);
		}

		[TestMethod]
		public async Task Tags()
		{
			List<Tag> result = await new ServiceTag(new RepositoryWebHDS(), new ServiceStory()).GetTags();

			GoldenMaster.Check(SOURCE, nameof(Tags), result, _handler);
		}

		private IServiceAuthor NewServiceAuthor()
		{
			return new ServiceAuthor(new RepositoryWebHDS(), new ServiceStory());
		}

		private Author NewAuthor()
		{
			return new Author()
			{
				Name = AUTHOR_NAME,
				Url = AUTHOR_URL
			};
		}
	}
}
