using Bunit;
using XXRead.Core.Models;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Constants;
using XXRead.UI.Pages;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class WelcomePageTests
	{
		[TestMethod]
		public async Task FirstRun_MainPageGoesToWelcomeWithoutLoading()
		{
			await using var context = new BunitContextUI();
			context.AppSettings.FirstRun = true;

			context.Render<MainPage>();

			Assert.AreEqual(typeof(WelcomePage), context.ServiceNavigation.CurrentPage);
			Assert.AreEqual(0, context.ServiceStory.InitStoriesCount);
		}

		[TestMethod]
		public async Task Welcome_ShowsXStoryAndHDS()
		{
			await using var context = new BunitContextUI();

			var page = context.Render<WelcomePage>();

			var buttons = page.FindAll(".welcome-data-source");
			Assert.AreEqual(2, buttons.Count);
			StringAssert.Contains(buttons[0].TextContent, WelcomePageConstants.WELCOMEPAGE_DATASOURCE_XSTORY);
			StringAssert.Contains(buttons[1].TextContent, WelcomePageConstants.WELCOMEPAGE_DATASOURCE_HDS);
		}

		[TestMethod]
		public async Task DataSourceClicked_SetsSourceThenGoesToMainPage()
		{
			await using var context = new BunitContextUI();
			context.AppSettings.FirstRun = true;
			var page = context.Render<WelcomePage>();

			page.FindAll(".welcome-data-source")[1].Click();

			Assert.AreEqual(DataSources.HDS, context.ServiceSettings.DataSource);
			Assert.IsFalse(context.AppSettings.FirstRun);
			Assert.AreEqual(typeof(MainPage), context.ServiceNavigation.CurrentPage);
		}
	}
}
