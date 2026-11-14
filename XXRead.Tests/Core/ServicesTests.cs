using Microsoft.Extensions.DependencyInjection;
using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core;
using XXRead.Core.Services;
using XXRead.Core.Session;
using XXRead.Core.Sources;
using XXRead.Core.Storage;

namespace XXRead.Tests.Core
{
	/// <summary>
	/// Core services (ex-BL.Common) : state kept by IServiceSession, current source read from IServiceSettings.
	/// Error paths are not tested : ServiceLog.Error writes into the logs database.
	/// </summary>
	[TestClass]
	public class ServicesTests
	{
		private ServiceSettingsMemory _serviceSettings;
		private ServiceSession _serviceSession;
		private SourceFake _sourceHDS;
		private SourceFake _sourceXStory;
		private ServiceSource _serviceSource;

		[TestInitialize]
		public void Init()
		{
			_serviceSettings = new ServiceSettingsMemory() { DataSource = DataSources.HDS };
			_serviceSession = new ServiceSession(_serviceSettings);
			_sourceHDS = new SourceFake(DataSources.HDS);
			_sourceXStory = new SourceFake(DataSources.XStory);
			_serviceSource = new ServiceSource(new List<ISource>() { _sourceHDS, _sourceXStory }, _serviceSession);
		}

		[TestMethod]
		public void ServiceConfig_DataSourceStoredInSettings()
		{
			ServiceConfig serviceConfig = new ServiceConfig(_serviceSession);

			serviceConfig.SetCurrentDataSource(DataSources.XStory);

			Assert.AreEqual(DataSources.XStory, _serviceSettings.DataSource);

			_serviceSettings.DataSource = DataSources.Demo;

			Assert.AreEqual(DataSources.Demo, serviceConfig.GetCurrentDataSource());
		}

		[TestMethod]
		public async Task ServiceStory_UsesCurrentSourcePageAndCategory()
		{
			ServiceStory serviceStory = new ServiceStory(_serviceSource, _serviceSession, new RepositoryStoryFake());
			new ServiceCategory(_serviceSource, _serviceSession, new RepositoryCategoryFake()).SetCurrentCategory(new Category() { Url = "/category" });

			await serviceStory.InitStories();
			await serviceStory.LoadMoreStories();

			CollectionAssert.AreEqual(new List<string>() { "1|/category", "2|/category" }, _sourceHDS.Calls);
			Assert.AreEqual(0, _sourceXStory.Calls.Count);

			// Changing the data source switches the source, without any manual synchronization
			_serviceSettings.DataSource = DataSources.XStory;
			serviceStory.ResetPageNumber();
			await serviceStory.InitStories();

			CollectionAssert.AreEqual(new List<string>() { "1|/category" }, _sourceXStory.Calls);
		}

		[TestMethod]
		public void ServiceStory_SharesStateThroughSession()
		{
			Story story = new Story() { Url = "/story" };

			new ServiceStory(_serviceSource, _serviceSession, new RepositoryStoryFake()).SetCurrentStory(story);

			Assert.AreSame(story, new ServiceStory(_serviceSource, _serviceSession, new RepositoryStoryFake()).GetCurrentStory());
		}

		[TestMethod]
		public void ServiceAuthor_UpdateAlreadyLoadedAuthor_ReplacesCachedAuthor()
		{
			ServiceAuthor serviceAuthor = new ServiceAuthor(_serviceSource, _serviceSession);
			Author author = new Author() { Url = "/author", Name = "Page 1" };
			Author authorUpdated = new Author() { Url = "/author", Name = "Page 2" };

			serviceAuthor.AddAlreadyLoadedAuthor(author);
			serviceAuthor.UpdateAlreadyLoadedAuthor(authorUpdated);

			Assert.AreSame(authorUpdated, serviceAuthor.GetAlreadyLoadedAuthor(new Author() { Url = "/author" }));
			Assert.AreEqual(1, _serviceSession.ListAlreadyLoadedAuthors.Count);
		}

		[TestMethod]
		public async Task ServiceAuthor_GetAuthorStoriesNextPage_UpdatesCache()
		{
			ServiceAuthor serviceAuthor = new ServiceAuthor(_serviceSource, _serviceSession);
			Author author = new Author() { Url = "/author" };
			serviceAuthor.AddAlreadyLoadedAuthor(author);

			Author nextPage = await serviceAuthor.GetAuthorStoriesNextPage(author);

			Assert.AreEqual("/author|2", nextPage.Name);
			Assert.AreSame(nextPage, serviceAuthor.GetAlreadyLoadedAuthor(author));
		}

		[TestMethod]
		public async Task ServiceAuthor_GetAuthorStoriesNextPage_PageNumberByAuthor()
		{
			ServiceAuthor serviceAuthor = new ServiceAuthor(_serviceSource, _serviceSession);
			Author authorA = new Author() { Url = "/authorA" };
			Author authorB = new Author() { Url = "/authorB" };

			Assert.AreEqual("/authorA|2", (await serviceAuthor.GetAuthorStoriesNextPage(authorA)).Name);
			Assert.AreEqual("/authorA|3", (await serviceAuthor.GetAuthorStoriesNextPage(authorA)).Name);

			// Another author starts at its own page 2
			Assert.AreEqual("/authorB|2", (await serviceAuthor.GetAuthorStoriesNextPage(authorB)).Name);

			// Back to the first author (cached) : continues where it stopped
			Assert.AreEqual("/authorA|4", (await serviceAuthor.GetAuthorStoriesNextPage(authorA)).Name);
		}

		[TestMethod]
		public async Task ServiceCategory_InitCategories_LoadsFromWebOnlyIfNotInDatabase()
		{
			RepositoryCategoryFake repositoryCategory = new RepositoryCategoryFake();
			ServiceCategory serviceCategory = new ServiceCategory(_serviceSource, _serviceSession, repositoryCategory);

			await serviceCategory.InitCategories();
			await serviceCategory.InitCategories();

			Assert.AreEqual(1, _sourceHDS.Calls.Count(c => c == "categories"));
			List<Category> categories = await serviceCategory.GetCategoriesDB();
			Assert.AreEqual("HDS", categories.Single().Source);
		}

		[TestMethod]
		public void AddXXReadCore_ResolvesServices()
		{
			ServiceCollection services = new ServiceCollection();
			services.AddXXReadCore();
			services.AddSingleton<IServiceSettings>(_serviceSettings);
			services.AddTransient<ISource>(_ => new SourceFake(DataSources.HDS));
			services.AddTransient<IRepositoryStory, RepositoryStoryFake>();
			services.AddTransient<IRepositoryCategory, RepositoryCategoryFake>();

			using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

			Assert.AreEqual(DataSources.HDS, provider.GetRequiredService<IServiceSource>().Current.Id);
			Assert.AreSame(provider.GetRequiredService<IServiceSession>(), provider.GetRequiredService<IServiceSession>());

			Story story = new Story() { Url = "/story" };
			provider.GetRequiredService<IServiceStory>().SetCurrentStory(story);

			Assert.AreSame(story, provider.GetRequiredService<IServiceStory>().GetCurrentStory());
			Assert.IsNotNull(provider.GetRequiredService<IServiceCategory>());
			Assert.IsNotNull(provider.GetRequiredService<IServiceAuthor>());
			Assert.AreEqual(DataSources.HDS, provider.GetRequiredService<IServiceConfig>().GetCurrentDataSource());
		}

		private class ServiceSettingsMemory : IServiceSettings
		{
			public DataSources DataSource { get; set; }
		}

		private class SourceFake : ISource
		{
			public SourceFake(DataSources id)
			{
				Id = id;
			}

			public DataSources Id { get; }

			public List<string> Calls { get; } = new List<string>();

			public Task<List<Story>> GetStoriesPage(int page, string categoryUrl = "", string sortCriterion = "")
			{
				Calls.Add($"{page}|{categoryUrl}");
				return Task.FromResult(new List<Story>() { new Story() { Url = $"/{Id}/{page}" } });
			}

			public Task<Author> GetAuthorPage(Author author, int pageNumber = 1)
			{
				return Task.FromResult(new Author() { Url = author.Url, Name = $"{author.Url}|{pageNumber}" });
			}

			public Task<List<Category>> GetCategories()
			{
				Calls.Add("categories");
				return Task.FromResult(new List<Category>() { new Category() { Title = "Category", Source = Id.ToString(), Url = "/category" } });
			}

			public Task<Story> GetStory(string url) => throw new NotImplementedException();
			public Task<List<Tag>> GetTags() => throw new NotImplementedException();
		}

		private class RepositoryCategoryFake : IRepositoryCategory
		{
			private readonly List<Category> _categories = new List<Category>();

			public Task<bool> HasCategories(DataSources source) => Task.FromResult(_categories.Any(c => c.Source == source.ToString()));
			public Task<List<Category>> GetCategories(DataSources source) => Task.FromResult(_categories.Where(c => c.Source == source.ToString()).ToList());

			public Task<int> InsertCategories(List<Category> categories)
			{
				_categories.AddRange(categories);
				return Task.FromResult(categories.Count);
			}
		}

		private class RepositoryStoryFake : IRepositoryStory
		{
			public Task<List<Story>> GetStories() => throw new NotImplementedException();
			public Task<Story> GetStory(string url) => throw new NotImplementedException();
			public Task<int> InsertStory(Story story) => throw new NotImplementedException();
			public Task<int> InsertStoryWithAuthorTransac(Story story) => throw new NotImplementedException();
			public Task<int> DeleteStory(Story story) => throw new NotImplementedException();
		}
	}
}
