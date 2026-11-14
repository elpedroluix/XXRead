using Microsoft.Extensions.DependencyInjection;
using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core.Sources;

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
		private ServiceProvider _serviceProvider;
		private ISource _source;

		[TestInitialize]
		public void Init()
		{
			_handler = new HttpHandlerFixtures(GoldenMaster.GetFixturesDirectory(SOURCE));
			_serviceProvider = GoldenMaster.BuildServiceProvider(_handler);
			_source = GoldenMaster.GetSource(_serviceProvider, DataSources.HDS);
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
		public async Task AuthorPage1()
		{
			await GoldenMaster.Check(SOURCE, nameof(AuthorPage1), () => _source.GetAuthorPage(NewAuthor()), _handler);
		}

		/// <summary>
		/// Same as the app: page 2 is loaded on the author of page 1 (the stories are added).
		/// </summary>
		[TestMethod]
		public async Task AuthorPage2()
		{
			await GoldenMaster.Check(SOURCE, nameof(AuthorPage2), async () =>
			{
				Author author = NewAuthor();

				await _source.GetAuthorPage(author);
				return await _source.GetAuthorPage(author, 2);
			}, _handler);
		}

		[TestMethod]
		public async Task AuthorLastPage()
		{
			await GoldenMaster.Check(SOURCE, nameof(AuthorLastPage), () => _source.GetAuthorPage(NewAuthor(), AUTHOR_LAST_PAGE), _handler);
		}

		[TestMethod]
		public async Task Categories()
		{
			await GoldenMaster.Check(SOURCE, nameof(Categories), () => _source.GetCategories(), _handler);
		}

		[TestMethod]
		public async Task Tags()
		{
			await GoldenMaster.Check(SOURCE, nameof(Tags), () => _source.GetTags(), _handler);
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
