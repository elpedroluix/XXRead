using Bunit;
using XXRead.Core.Models;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Constants;
using XXRead.UI.Pages;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class MainPageTests
	{
		[TestMethod]
		public void StoriesPending_ShowsLoading()
		{
			using var context = new BunitContextUI();
			var pending = new TaskCompletionSource<List<Story>>();
			context.ServiceStory.InitStoriesResult = () => pending.Task;

			var page = context.Render<MainPage>();

			StringAssert.Contains(page.Markup, GlobalConstants.LOADING);
		}

		[TestMethod]
		public void StoriesError_ShowsErrorThenTryAgainReloads()
		{
			using var context = new BunitContextUI();
			context.ServiceStory.InitStoriesResult = () => Task.FromResult<List<Story>>(null);
			var page = context.Render<MainPage>();
			StringAssert.Contains(page.Markup, GlobalConstants.ERROR_MESSAGE);

			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Story 1"));
			page.FindAll("button").Single(button => button.TextContent.Contains(GlobalConstants.ERROR_TRYAGAIN)).Click();

			page.WaitForAssertion(() => StringAssert.Contains(page.Markup, "Story 1"));
			Assert.AreEqual(2, context.ServiceStory.InitStoriesCount);
		}

		[TestMethod]
		public void NoStories_ShowsEmptyMessage()
		{
			using var context = new BunitContextUI();

			var page = context.Render<MainPage>();

			StringAssert.Contains(page.Markup, MainPageConstants.MAINPAGE_NO_STORIES);
			Assert.IsFalse(page.Markup.Contains(MainPageConstants.MAINPAGE_LOAD_MORE));
		}

		[TestMethod]
		public void Stories_ShowsCardsCategoryAndLoadMore()
		{
			using var context = new BunitContextUI();
			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Story 1", "Story 2"));
			context.ServiceCategory.CurrentCategory = new Category() { Title = "Gay" };

			var page = context.Render<MainPage>();

			Assert.AreEqual(2, page.FindAll(".story-card").Count);
			StringAssert.Contains(page.Markup, "Story 2");
			StringAssert.Contains(page.Markup, "Gay");
			StringAssert.Contains(page.Markup, MainPageConstants.MAINPAGE_LOAD_MORE);
		}

		[TestMethod]
		public void StoryClicked_OpensStoryPage()
		{
			using var context = new BunitContextUI();
			List<Story> stories = CreateStories("Story 1", "Story 2");
			context.ServiceStory.InitStoriesResult = () => Task.FromResult(stories);
			var page = context.Render<MainPage>();

			page.FindAll(".story-card")[1].Click();

			page.WaitForAssertion(() => Assert.AreEqual(typeof(StoryPage), context.ServiceNavigation.CurrentPage));
			Assert.AreSame(stories[1], context.ServiceStory.CurrentStory);
		}

		[TestMethod]
		public void BackOnPage_KeepsStories()
		{
			using var context = new BunitContextUI();
			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Story 1"));
			context.Render<MainPage>().Instance.Dispose();

			var page = context.Render<MainPage>();

			StringAssert.Contains(page.Markup, "Story 1");
			Assert.AreEqual(1, context.ServiceStory.InitStoriesCount);
		}

		[TestMethod]
		public void BackOnPageAfterDataSourceChanged_ReloadsStories()
		{
			using var context = new BunitContextUI();
			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Story 1"));
			context.Render<MainPage>().Instance.Dispose();

			context.AppSettings.DataSourceChanged = true;
			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Other source"));
			var page = context.Render<MainPage>();

			StringAssert.Contains(page.Markup, "Other source");
			Assert.AreEqual(2, context.ServiceStory.InitStoriesCount);
			Assert.IsFalse(context.AppSettings.DataSourceChanged);
		}

		private static List<Story> CreateStories(params string[] titles)
		{
			return titles.Select(title => new Story() { Title = title, Url = title }).ToList();
		}
	}
}
