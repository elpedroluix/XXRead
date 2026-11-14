using Microsoft.Extensions.DependencyInjection;
using XXRead.Core.Models;
using XXRead.Core.Sources;

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
		private ServiceProvider _serviceProvider;
		private ISource _source;

		[TestInitialize]
		public void Init()
		{
			_handler = new HttpHandlerFixtures(GoldenMaster.GetFixturesDirectory(SOURCE));
			_serviceProvider = GoldenMaster.BuildServiceProvider(_handler);
			_source = GoldenMaster.GetSource(_serviceProvider, DataSources.XStory);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_serviceProvider.Dispose();
		}

		[TestMethod]
		public async Task StoriesPage1()
		{
			await GoldenMaster.Check(SOURCE, nameof(StoriesPage1), () => _source.GetStoriesPage(0), _handler);
		}

		[TestMethod]
		public async Task StoriesPage2()
		{
			await GoldenMaster.Check(SOURCE, nameof(StoriesPage2), () => _source.GetStoriesPage(2), _handler);
		}

		[TestMethod]
		public async Task StoriesCategoryPage1()
		{
			await GoldenMaster.Check(SOURCE, nameof(StoriesCategoryPage1), () => _source.GetStoriesPage(0, CATEGORY_URL), _handler);
		}

		[TestMethod]
		public async Task StoriesCategoryPage2()
		{
			await GoldenMaster.Check(SOURCE, nameof(StoriesCategoryPage2), () => _source.GetStoriesPage(2, CATEGORY_URL), _handler);
		}

		[TestMethod]
		public async Task Story1()
		{
			await GoldenMaster.Check(SOURCE, nameof(Story1), () => _source.GetStory(STORY_URL_1), _handler);
		}

		[TestMethod]
		public async Task Story2()
		{
			await GoldenMaster.Check(SOURCE, nameof(Story2), () => _source.GetStory(STORY_URL_2), _handler);
		}

		[TestMethod]
		public async Task AuthorPage()
		{
			Author author = new Author()
			{
				Name = AUTHOR_NAME,
				Url = AUTHOR_URL
			};

			await GoldenMaster.Check(SOURCE, nameof(AuthorPage), () => _source.GetAuthorPage(author), _handler);
		}

		[TestMethod]
		public async Task Categories()
		{
			await GoldenMaster.Check(SOURCE, nameof(Categories), () => _source.GetCategories(), _handler);
		}
	}
}
