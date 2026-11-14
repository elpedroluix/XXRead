using Bunit;
using XXRead.Logger;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Constants;
using XXRead.UI.Pages;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class LogsPageTests
	{
		[TestMethod]
		public async Task Logs_ShowsMostRecentFirst()
		{
			await using var context = new BunitContextUI();
			context.ServiceLogs.Logs = new List<Log>()
			{
				CreateLog("Old error", "2026-11-01 10:00:00"),
				CreateLog("Recent error", "2026-11-14 09:00:00"),
			};

			var page = context.Render<LogsPage>();

			var items = page.FindAll(".log-item");
			Assert.AreEqual(2, items.Count);
			StringAssert.Contains(items[0].TextContent, "Recent error");
			StringAssert.Contains(items[1].TextContent, "Old error");
		}

		[TestMethod]
		public async Task NoLogs_ShowsEmptyMessage()
		{
			await using var context = new BunitContextUI();

			var page = context.Render<LogsPage>();

			StringAssert.Contains(page.Markup, SettingsPageConstants.SETTINGS_LOGS_EMPTY);
		}

		[TestMethod]
		public async Task LogsError_ShowsErrorThenTryAgainReloads()
		{
			await using var context = new BunitContextUI();
			context.ServiceLogs.Logs = null;
			var page = context.Render<LogsPage>();
			StringAssert.Contains(page.Markup, GlobalConstants.ERROR_MESSAGE);

			context.ServiceLogs.Logs = new List<Log>() { CreateLog("Error", "2026-11-14 09:00:00") };
			page.FindAll("button").Single(button => button.TextContent.Contains(GlobalConstants.ERROR_TRYAGAIN)).Click();

			page.WaitForAssertion(() => Assert.AreEqual(1, page.FindAll(".log-item").Count));
			Assert.AreEqual(2, context.ServiceLogs.GetLogsCount);
		}

		private static Log CreateLog(string content, string date)
		{
			return new Log() { Id = Guid.NewGuid(), Content = content, Date = date, Type = LogType.Error.ToString() };
		}
	}
}
