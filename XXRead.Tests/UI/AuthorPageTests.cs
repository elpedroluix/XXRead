using Bunit;
using MudBlazor;
using XXRead.Core.Models;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Constants;
using XXRead.UI.Pages;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class AuthorPageTests
	{
		private const string AUTHOR_URL = "author/1";

		[TestMethod]
		public async Task AuthorPending_ShowsLoading()
		{
			await using var context = CreateContext();
			var pending = new TaskCompletionSource<Author>();
			context.ServiceAuthor.InitAuthorResult = () => pending.Task;

			var page = context.Render<AuthorPage>();

			StringAssert.Contains(page.Markup, GlobalConstants.LOADING);
		}

		[TestMethod]
		public async Task AuthorError_ShowsErrorThenTryAgainReloads()
		{
			await using var context = CreateContext();
			context.ServiceAuthor.InitAuthorResult = () => Task.FromResult<Author>(null);
			var page = context.Render<AuthorPage>();
			StringAssert.Contains(page.Markup, GlobalConstants.ERROR_MESSAGE);

			context.ServiceAuthor.InitAuthorResult = () => Task.FromResult(CreateAuthor());
			page.FindAll("button").Single(button => button.TextContent.Contains(GlobalConstants.ERROR_TRYAGAIN)).Click();

			page.WaitForAssertion(() => Assert.AreEqual("Auteur", page.Find(".author-name").TextContent));
			Assert.AreEqual(2, context.ServiceAuthor.InitAuthorCount);
		}

		[TestMethod]
		public async Task HDS_ShowsHDSInfos()
		{
			await using var context = CreateContext();
			context.ServiceSettings.DataSource = DataSources.HDS;

			var page = context.Render<AuthorPage>();

			StringAssert.Contains(page.Markup, AuthorPageConstants.AUTHORPAGE_HDS_SLOGAN);
			StringAssert.Contains(page.Markup, "Mon slogan");
			Assert.IsFalse(page.Markup.Contains(AuthorPageConstants.AUTHORPAGE_XSTORY_STATUS));
		}

		[TestMethod]
		public async Task OtherSource_ShowsXStoryInfos()
		{
			await using var context = CreateContext();
			context.ServiceSettings.DataSource = DataSources.XStory;

			var page = context.Render<AuthorPage>();

			StringAssert.Contains(page.Markup, AuthorPageConstants.AUTHORPAGE_XSTORY_STATUS);
			Assert.IsFalse(page.Markup.Contains(AuthorPageConstants.AUTHORPAGE_HDS_SLOGAN));
		}

		[TestMethod]
		public async Task AlreadyLoadedAuthor_NotLoadedAgain()
		{
			await using var context = CreateContext();
			context.ServiceAuthor.AlreadyLoadedAuthors.Add(CreateAuthor());

			var page = context.Render<AuthorPage>();

			Assert.AreEqual("Auteur", page.Find(".author-name").TextContent);
			Assert.AreEqual(0, context.ServiceAuthor.InitAuthorCount);
		}

		[TestMethod]
		public async Task StoriesTab_ShowsStoriesWithoutLoadMore()
		{
			await using var context = CreateContext();
			var page = context.Render<AuthorPage>();

			OpenStoriesTab(page);

			Assert.AreEqual(2, page.FindAll(".story-card").Count);
			Assert.AreEqual(0, page.FindAll(".author-load-more").Count);
		}

		[TestMethod]
		public async Task NoStories_ShowsEmptyMessage()
		{
			await using var context = CreateContext();
			context.ServiceAuthor.SetCurrentAuthor(new Author() { Name = "Auteur", Url = AUTHOR_URL });
			var page = context.Render<AuthorPage>();

			OpenStoriesTab(page);

			StringAssert.Contains(page.Markup, AuthorPageConstants.AUTHORPAGE_NO_STORIES);
		}

		[TestMethod]
		public async Task LoadMoreClicked_ShowsNextPageAndHidesButtonOnLastPage()
		{
			await using var context = CreateContext();
			context.ServiceAuthor.CurrentAuthor.HasMorePages = true;
			context.ServiceAuthor.NextPageResult = author =>
			{
				author.Stories.Add(new Story() { Title = "Story 3", Url = "story/3" });
				author.HasMorePages = false;
				return Task.FromResult(author);
			};
			var page = context.Render<AuthorPage>();
			OpenStoriesTab(page);

			page.Find(".author-load-more").Click();

			page.WaitForAssertion(() => Assert.AreEqual(3, page.FindAll(".story-card").Count));
			Assert.AreEqual(0, page.FindAll(".author-load-more").Count);
			Assert.AreEqual(1, context.ServiceAuthor.NextPageCount);
		}

		[TestMethod]
		public async Task StoryClicked_OpensStory()
		{
			await using var context = CreateContext();
			var page = context.Render<AuthorPage>();
			OpenStoriesTab(page);

			page.FindAll(".story-card")[1].Click();

			page.WaitForAssertion(() => Assert.AreEqual(typeof(StoryPage), context.ServiceNavigation.CurrentPage));
			Assert.AreEqual("Story 2", context.ServiceStory.CurrentStory.Title);
		}

		[TestMethod]
		public async Task StoryWithChapters_ChapterSelectedInDialogIsOpened()
		{
			await using var context = CreateContext();
			context.ServiceAuthor.CurrentAuthor.Stories[0].ChaptersList = new List<Story>()
			{
				new Story() { Title = "Chapitre 1", Url = "story/1/1" },
				new Story() { Title = "Chapitre 2", Url = "story/1/2" },
			};
			var provider = context.Render<MudDialogProvider>();
			var page = context.Render<AuthorPage>();
			OpenStoriesTab(page);

			await page.InvokeAsync(() => { page.FindAll(".story-card")[0].Click(); });
			provider.WaitForAssertion(() => Assert.AreEqual(2, provider.FindAll(".mud-list-item").Count));
			provider.FindAll(".mud-list-item")[1].Click();

			page.WaitForAssertion(() => Assert.AreEqual(typeof(StoryPage), context.ServiceNavigation.CurrentPage));
			Assert.AreEqual("Chapitre 2", context.ServiceStory.CurrentStory.Title);
		}

		[TestMethod]
		public async Task BackWithSameAuthor_PageKeptAsLeft()
		{
			await using var context = CreateContext();
			var firstPage = context.Render<AuthorPage>();
			OpenStoriesTab(firstPage);
			firstPage.Dispose();

			var page = context.Render<AuthorPage>();

			Assert.AreEqual(2, page.FindAll(".story-card").Count);
			Assert.AreEqual(1, context.ServiceAuthor.InitAuthorCount);
		}

		[TestMethod]
		public async Task OtherAuthor_LoadedOnInfosTab()
		{
			await using var context = CreateContext();
			var firstPage = context.Render<AuthorPage>();
			OpenStoriesTab(firstPage);
			firstPage.Dispose();

			context.ServiceAuthor.SetCurrentAuthor(new Author() { Name = "Autre", Url = "author/2" });
			var page = context.Render<AuthorPage>();

			Assert.AreEqual("Autre", page.Find(".author-name").TextContent);
			Assert.AreEqual(2, context.ServiceAuthor.InitAuthorCount);
		}

		/// <summary>
		/// The current author has 2 stories ; InitAuthor returns the current author.
		/// </summary>
		private static BunitContextUI CreateContext()
		{
			var context = new BunitContextUI();
			context.ServiceAuthor.SetCurrentAuthor(CreateAuthor());

			return context;
		}

		private static Author CreateAuthor()
		{
			var author = new Author() { Name = "Auteur", Url = AUTHOR_URL, Slogan = "Mon slogan" };
			author.Stories.Add(new Story() { Title = "Story 1", Url = "story/1" });
			author.Stories.Add(new Story() { Title = "Story 2", Url = "story/2" });

			return author;
		}

		private static void OpenStoriesTab(IRenderedComponent<AuthorPage> page)
		{
			page.FindAll(".mud-tab").Single(tab => tab.TextContent.Contains(AuthorPageConstants.AUTHORPAGE_TAB_STORIES)).Click();
		}
	}
}
