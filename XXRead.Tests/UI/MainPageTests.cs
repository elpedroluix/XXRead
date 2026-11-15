using Bunit;
using MudBlazor;
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

		[TestMethod]
		public async Task CategoryChanged_ReloadsStories()
		{
			await using var context = new BunitContextUI();
			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Story 1"));
			Category gay = new Category() { Title = "Gay" };
			context.ServiceCategory.Categories = new List<Category>() { gay };
			var provider = context.Render<MudDialogProvider>();
			var page = context.Render<MainPage>();

			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Gay story"));
			SelectCategory(page, provider, "Gay");

			page.WaitForAssertion(() => StringAssert.Contains(page.Markup, "Gay story"));
			Assert.AreSame(gay, context.ServiceCategory.CurrentCategory);
			Assert.AreEqual(2, context.ServiceStory.InitStoriesCount);
		}

		[TestMethod]
		public async Task CategoryChangedError_ShowsError()
		{
			await using var context = new BunitContextUI();
			context.ServiceStory.InitStoriesResult = () => Task.FromResult(CreateStories("Story 1"));
			context.ServiceCategory.Categories = new List<Category>() { new Category() { Title = "Gay" } };
			var provider = context.Render<MudDialogProvider>();
			var page = context.Render<MainPage>();

			context.ServiceStory.InitStoriesResult = () => Task.FromResult<List<Story>>(null);
			SelectCategory(page, provider, "Gay");

			page.WaitForAssertion(() => StringAssert.Contains(page.Markup, GlobalConstants.ERROR_MESSAGE));
		}

		private static void SelectCategory(IRenderedComponent<MainPage> page, IRenderedComponent<MudDialogProvider> provider, string title)
		{
			page.Find(".category-control").Click();
			provider.WaitForAssertion(() => Assert.AreEqual(2, provider.FindAll(".mud-list-item").Count));
			provider.FindAll(".mud-list-item").Single(item => item.TextContent.Contains(title)).Click();
		}

		private static List<Story> CreateStories(params string[] titles)
		{
			return titles.Select(title => new Story() { Title = title, Url = title }).ToList();
		}
	}
}
