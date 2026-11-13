using Microsoft.Extensions.DependencyInjection;
using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core;
using XXRead.Core.Session;
using XXRead.Core.Sources;

namespace XXRead.Tests.Core
{
	[TestClass]
	public class ServiceSourceTests
	{
		[TestMethod]
		public void Current_FollowsSessionDataSource()
		{
			ServiceSettingsMemory serviceSettings = new ServiceSettingsMemory() { DataSource = DataSources.HDS };
			ServiceSession serviceSession = new ServiceSession(serviceSettings);
			ServiceSource serviceSource = new ServiceSource(
				new List<ISource>() { new SourceFake(DataSources.XStory), new SourceFake(DataSources.HDS) },
				serviceSession);

			Assert.AreEqual(DataSources.HDS, serviceSource.Current.Id);

			serviceSession.DataSource = DataSources.XStory;

			Assert.AreEqual(DataSources.XStory, serviceSource.Current.Id);
			Assert.AreEqual(DataSources.XStory, serviceSettings.DataSource);
		}

		[TestMethod]
		public void Get_KO_SourceNotRegistered()
		{
			ServiceSource serviceSource = new ServiceSource(
				new List<ISource>() { new SourceFake(DataSources.XStory) },
				new ServiceSession(new ServiceSettingsMemory()));

			Assert.ThrowsException<InvalidOperationException>(() => serviceSource.Get(DataSources.Demo));
		}

		[TestMethod]
		public void Get_KO_SourceRegisteredTwice()
		{
			ServiceSource serviceSource = new ServiceSource(
				new List<ISource>() { new SourceFake(DataSources.HDS), new SourceFake(DataSources.HDS) },
				new ServiceSession(new ServiceSettingsMemory()));

			Assert.ThrowsException<InvalidOperationException>(() => serviceSource.Get(DataSources.HDS));
		}

		[TestMethod]
		public void AddXXReadCore_ResolvesServices()
		{
			ServiceCollection services = new ServiceCollection();
			services.AddXXReadCore();
			services.AddSingleton<IServiceSettings>(new ServiceSettingsMemory() { DataSource = DataSources.Demo });
			services.AddTransient<ISource>(_ => new SourceFake(DataSources.Demo));

			using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions() { ValidateOnBuild = true, ValidateScopes = true });

			Assert.AreEqual(DataSources.Demo, provider.GetRequiredService<IServiceSource>().Current.Id);
			Assert.AreSame(provider.GetRequiredService<IServiceSession>(), provider.GetRequiredService<IServiceSession>());
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

			public Task<List<Story>> GetStoriesPage(int page, string categoryUrl = "", string sortCriterion = "") => throw new NotImplementedException();
			public Task<Story> GetStory(string url) => throw new NotImplementedException();
			public Task<Author> GetAuthorPage(Author author, int pageNumber = 1) => throw new NotImplementedException();
			public Task<List<Category>> GetCategories() => throw new NotImplementedException();
			public Task<List<Tag>> GetTags() => throw new NotImplementedException();
		}
	}
}
