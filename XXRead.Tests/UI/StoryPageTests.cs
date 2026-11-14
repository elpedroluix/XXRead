using Bunit;
using MudBlazor;
using XXRead.Core.Models;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Constants;
using XXRead.UI.Pages;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class StoryPageTests
	{
		private const int CHAPTERS_COUNT = 2;

		[TestMethod]
		public void StoryPending_ShowsLoading()
		{
			using var context = CreateContext();
			var pending = new TaskCompletionSource<Story>();
			context.ServiceStory.InitStoryResult = () => pending.Task;

			var page = context.Render<StoryPage>();

			StringAssert.Contains(page.Markup, GlobalConstants.LOADING);
		}

		[TestMethod]
		public void StoryError_ShowsErrorThenTryAgainReloads()
		{
			using var context = CreateContext();
			context.ServiceStory.InitStoryResult = () => Task.FromResult<Story>(null);
			var page = context.Render<StoryPage>();
			StringAssert.Contains(page.Markup, GlobalConstants.ERROR_MESSAGE);

			context.ServiceStory.InitStoryResult = () => Task.FromResult(CreateChapter(1));
			page.FindAll("button").Single(button => button.TextContent.Contains(GlobalConstants.ERROR_TRYAGAIN)).Click();

			page.WaitForAssertion(() => Assert.AreEqual("Content 1", page.Find(".story-content").TextContent));
			Assert.AreEqual(2, context.ServiceStory.InitStoryCount);
		}

		[TestMethod]
		public void FirstChapter_ShowsStoryAndNextArrowOnly()
		{
			using var context = CreateContext();

			var page = context.Render<StoryPage>();

			Assert.AreEqual("Content 1", page.Find(".story-content").TextContent);
			StringAssert.Contains(page.Find(".chapter-name").TextContent, "Chapitre 1");
			StringAssert.Contains(page.Markup, "Auteur");
			Assert.AreEqual(0, page.FindAll(".chapter-previous").Count);
			Assert.AreEqual(1, page.FindAll(".chapter-next").Count);
		}

		[TestMethod]
		public void NextChapterClicked_LoadsNextChapter()
		{
			using var context = CreateContext();
			var page = context.Render<StoryPage>();

			page.Find(".chapter-next").Click();

			page.WaitForAssertion(() => Assert.AreEqual("Content 2", page.Find(".story-content").TextContent));
			Assert.AreEqual(2, context.ServiceStory.CurrentStory.ChapterNumber);
			Assert.AreEqual(1, page.FindAll(".chapter-previous").Count);
			Assert.AreEqual(0, page.FindAll(".chapter-next").Count);
		}

		[TestMethod]
		public void AlreadyLoadedStory_NotLoadedAgain()
		{
			using var context = CreateContext();
			context.ServiceStory.AlreadyLoadedStories.Add(CreateChapter(1));

			var page = context.Render<StoryPage>();

			Assert.AreEqual("Content 1", page.Find(".story-content").TextContent);
			Assert.AreEqual(0, context.ServiceStory.InitStoryCount);
		}

		[TestMethod]
		public void SaveClicked_SavesStoryAndShowsDelete()
		{
			using var context = CreateContext();
			var page = context.Render<StoryPage>();

			FindButton(page, StoryPageConstants.STORYPAGE_SAVE).Click();

			page.WaitForAssertion(() => FindButton(page, StoryPageConstants.STORYPAGE_DELETE));
			Assert.AreEqual(1, context.ServiceStory.SavedStories.Count);
		}

		[TestMethod]
		public void AuthorClicked_OpensAuthorPage()
		{
			using var context = CreateContext();
			var page = context.Render<StoryPage>();

			page.Find(".story-author").Click();

			Assert.AreEqual("Auteur", context.ServiceAuthor.CurrentAuthor.Name);
			Assert.AreEqual(typeof(AuthorPage), context.ServiceNavigation.CurrentPage);
		}

		[TestMethod]
		public void NoAuthor_AuthorClickedDoesNothing()
		{
			using var context = CreateContext();
			context.ServiceStory.InitStoryResult = () =>
			{
				Story story = CreateChapter(1);
				story.Author = null;
				return Task.FromResult(story);
			};
			var page = context.Render<StoryPage>();

			page.Find(".story-author").Click();

			Assert.IsNull(context.ServiceAuthor.CurrentAuthor);
			Assert.IsNull(context.ServiceNavigation.CurrentPage);
		}

		[TestMethod]
		public async Task ChapterSelectedInDialog_LoadsChapter()
		{
			await using var context = CreateContext();
			var provider = context.Render<MudDialogProvider>();
			var page = context.Render<StoryPage>();

			await page.InvokeAsync(() => page.Find(".chapter-name").Click());
			provider.WaitForAssertion(() => Assert.AreEqual(CHAPTERS_COUNT, provider.FindAll(".mud-list-item").Count));
			provider.FindAll(".mud-list-item")[1].Click();

			page.WaitForAssertion(() => Assert.AreEqual("Content 2", page.Find(".story-content").TextContent));
		}

		/// <summary>
		/// The current story is the 1st chapter ; InitStory returns the current chapter, with its content.
		/// </summary>
		private static BunitContextUI CreateContext()
		{
			var context = new BunitContextUI();
			context.ServiceStory.SetCurrentStory(CreateChapter(1));
			context.ServiceStory.InitStoryResult = () => Task.FromResult(CreateChapter(context.ServiceStory.CurrentStory.ChapterNumber));

			return context;
		}

		private static Story CreateChapter(int chapterNumber)
		{
			List<Story> chapters = Enumerable.Range(1, CHAPTERS_COUNT)
				.Select(number => new Story() { Title = "Chapitre " + number, Url = "story/" + number, ChapterNumber = number })
				.ToList();

			return new Story()
			{
				Title = "Story",
				Url = "story/" + chapterNumber,
				ChapterNumber = chapterNumber,
				ChapterName = "Chapitre " + chapterNumber,
				Content = "Content " + chapterNumber,
				ChaptersList = chapters,
				Author = new Author() { Name = "Auteur" },
			};
		}

		private static AngleSharp.Dom.IElement FindButton(IRenderedComponent<StoryPage> page, string text)
		{
			return page.FindAll("button").Single(button => button.TextContent.Trim() == text);
		}
	}
}
