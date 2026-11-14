using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using XXRead.Core.Models;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Constants;
using XXRead.UI.Pages;
using XXRead.UI.Services;
using XXRead.UI.Theming;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class SettingsPageTests
	{
		[TestMethod]
		public async Task CurrentSettings_ShowsSourceAndSelectedColors()
		{
			await using var context = new BunitContextUI();

			var page = context.Render<SettingsPage>();

			StringAssert.Contains(page.Find(".settings-data-source").TextContent, DataSources.Demo.ToString());
			var selected = page.FindAll(".theme-swatch-selected");
			Assert.AreEqual(2, selected.Count);
			Assert.AreEqual(ThemeColors.Dark.Name, selected[0].GetAttribute("title"));
			Assert.AreEqual(ThemeColors.MainRed, selected[1].GetAttribute("title"));
		}

		[TestMethod]
		public async Task OtherSourceSelected_ChangesSourceAndResetsCategory()
		{
			await using var context = new BunitContextUI();
			context.ServiceCategory.CurrentCategory = new Category() { Title = "Gay" };
			var provider = context.Render<MudDialogProvider>();
			var page = context.Render<SettingsPage>();

			await OpenDataSourceDialog(page, provider);
			provider.FindAll(".mud-list-item").Single(item => item.TextContent.Contains(DataSources.HDS.ToString())).Click();

			page.WaitForAssertion(() => StringAssert.Contains(page.Find(".settings-data-source").TextContent, DataSources.HDS.ToString()));
			Assert.AreEqual(DataSources.HDS, context.ServiceSettings.DataSource);
			Assert.IsNull(context.ServiceCategory.CurrentCategory);
			Assert.IsTrue(context.AppSettings.DataSourceChanged);
		}

		[TestMethod]
		public async Task DataSourceDialogClosed_KeepsSource()
		{
			await using var context = new BunitContextUI();
			var provider = context.Render<MudDialogProvider>();
			var page = context.Render<SettingsPage>();

			await OpenDataSourceDialog(page, provider);
			provider.FindAll("button").Single(button => button.TextContent.Contains(GlobalConstants.CLOSE)).Click();

			provider.WaitForAssertion(() => Assert.AreEqual(0, provider.FindAll(".mud-list-item").Count));
			Assert.AreEqual(DataSources.Demo, context.ServiceSettings.DataSource);
			Assert.IsFalse(context.AppSettings.DataSourceChanged);
		}

		[TestMethod]
		public async Task LightClicked_AppliesLightTheme()
		{
			await using var context = new BunitContextUI();
			var page = context.Render<SettingsPage>();

			page.Find($".theme-swatch[title='{ThemeColors.Light.Name}']").Click();

			Assert.AreEqual(ThemeColors.LightPrimary, context.AppSettings.ThemePrimary);
			Assert.AreEqual(ThemeColors.FontDarkPrimary, context.AppSettings.ThemeFontPrimary);
			Assert.IsFalse(context.Services.GetRequiredService<IServiceTheme>().IsDarkMode);
			Assert.AreEqual(ThemeColors.Light.Name, page.FindAll(".theme-swatch-selected")[0].GetAttribute("title"));
		}

		[TestMethod]
		public async Task MainColorClicked_AppliesMainColor()
		{
			await using var context = new BunitContextUI();
			var page = context.Render<SettingsPage>();

			page.Find($".theme-swatch[title='{ThemeColors.MainPink}']").Click();

			Assert.AreEqual(ThemeColors.MainPink, context.AppSettings.ThemeMain);
			Assert.AreEqual(ThemeColors.MainPink, context.Services.GetRequiredService<IServiceTheme>().StatusBarColor);
			Assert.AreEqual(ThemeColors.MainPink, page.FindAll(".theme-swatch-selected")[1].GetAttribute("title"));
		}

		[TestMethod]
		public async Task LogsClicked_OpensLogsPage()
		{
			await using var context = new BunitContextUI();
			var page = context.Render<SettingsPage>();

			page.Find(".settings-logs").Click();

			Assert.AreEqual(typeof(LogsPage), context.ServiceNavigation.CurrentPage);
		}

		/// <summary>
		/// The dialog lists the two other sources.
		/// </summary>
		private static async Task OpenDataSourceDialog(IRenderedComponent<SettingsPage> page, IRenderedComponent<MudDialogProvider> provider)
		{
			// Statement lambda : InvokeAsync must not await the dialog result
			await page.InvokeAsync(() => { page.Find(".settings-data-source").Click(); });
			provider.WaitForAssertion(() => Assert.AreEqual(2, provider.FindAll(".mud-list-item").Count));
		}
	}
}
