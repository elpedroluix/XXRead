using XStory.BL.Web.XStory;
using XStory.DAL.Web.XStory;
using XStory.DTO;

namespace XXRead.Tests.GoldenMaster
{
	/// <summary>
	/// XStory scraping on saved HTML pages, compared to the reference JSON (see <see cref="GoldenMaster"/>).
	/// </summary>
	[TestClass]
	public class GoldenMasterXStoryTests
	{
		private const string SOURCE = "XStory";

		private const string CATEGORY_URL = "https://www.xstory-fr.com/histoires-erotiques/hetero";

		// Stories with several chapters (7 and 2 when the fixtures were recorded)
		private const string STORY_URL_1 = "https://www.xstory-fr.com/histoires-erotiques/01M48YS6E7GP1N0NX9FMF4NV0M-en-vacances-chez-mon-oncle/chapitres/01M48YS6E96WQZN1JE8E1JJSPQ-premiere-soiree";
		private const string STORY_URL_2 = "https://www.xstory-fr.com/histoires-erotiques/01M74QGJRF37NW90QZ1CCJ6DXW-les-petits-secrets-damandine-au-coin-du-feu/chapitres/01M769S90EQG131JXJ29GFFNT2-du-point-de-vue-de-didier";

		private const string AUTHOR_NAME = "MarieBoobs";
		private const string AUTHOR_URL = "https://www.xstory-fr.com/utilisateurs/marieboobs";

		private HttpHandlerFixtures _handler;
		private HttpClient _originalHttpClient;

		[TestInitialize]
		public void Init()
		{
			_handler = new HttpHandlerFixtures(GoldenMaster.GetFixturesDirectory(SOURCE));
			_originalHttpClient = GoldenMaster.SwapHttpClient(typeof(RepositoryWebXStory), new RepositoryWebXStory().GetHttpClient(), _handler);
		}

		[TestCleanup]
		public void Cleanup()
		{
			GoldenMaster.RestoreHttpClient(typeof(RepositoryWebXStory), _originalHttpClient);
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
		public async Task AuthorPage()
		{
			Author author = new Author()
			{
				Name = AUTHOR_NAME,
				Url = AUTHOR_URL
			};

			Author result = await new ServiceAuthor().GetAuthorPage(author);

			GoldenMaster.Check(SOURCE, nameof(AuthorPage), result, _handler);
		}

		[TestMethod]
		public async Task Categories()
		{
			List<Category> result = await new ServiceCategory().GetCategories();

			GoldenMaster.Check(SOURCE, nameof(Categories), result, _handler);
		}
	}
}
