using XXRead.Core.Models;
using XXRead.Core.Sources;

namespace XXRead.Tests.XStory
{
	[TestClass]
	public class StoryTests
	{
		[TestMethod]
		public void GetStoriesMainPageTest_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			int page = 0;

			Task<List<Story>> task = _source.GetStoriesPage(page);
			var result = task.Result;

			//string json = System.Text.Json.JsonSerializer.Serialize(result);

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStory_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,femme-est-une-chienne,53291.html");
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStory_OK_01_TypeAucun()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,garcon-timide-fille-epanouie,56348.html");
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStory_OK_02_TypeFantasme()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,the-walking-dicks,56341.html");
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStory_OK_03_TypeHistoireVraie()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,ami-gay-copine,56384.html");
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStory_OK_04_ViewsNumber()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,des-vacances-salee,56429.html");
			var result = task.Result;

			Assert.IsNotNull(result);
		}



		[TestMethod]
		public void GetStoryWithChapters_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,fiona-journaliste-sportive,46905.html");
			var result = task.Result;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.ChaptersList);
		}

		[TestMethod]
		public void GetStoryWithChaptersJSON_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,fiona-journaliste-sportive,46905.html");
			var result = task.Result;

			string json = System.Text.Json.JsonSerializer.Serialize(result);

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.ChaptersList);
		}

		[TestMethod]
		public void GetStoryWithNotAllChaptersNamed_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,tante-soeur,54397.html");
			var result = task.Result;

			Assert.IsNotNull(result);
			foreach (var chapter in result.ChaptersList)
			{
				Assert.AreNotEqual("", chapter.Title);
			}
		}


		[TestMethod]
		public void GetStoryMostViewed_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("http://xstory-fr.com/lire-histoire,sauvetage-une-maman,49583.html");
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStoryUglyCharacters_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Task<Story> task = _source.GetStory("https://www.xstory-fr.com/lire-histoire,amanda-seule,65771.html");
			var result = task.Result;

			Assert.IsNotNull(result);
		}
	}
}