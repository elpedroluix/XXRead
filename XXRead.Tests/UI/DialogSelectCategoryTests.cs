using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using XXRead.Core.Models;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Constants;
using XXRead.UI.Dialogs;
using XXRead.UI.Services;

namespace XXRead.Tests.UI
{
	[TestClass]
	public class DialogSelectCategoryTests
	{
		private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(5);

		[TestMethod]
		public async Task CategoryClicked_ReturnsCategory()
		{
			await using var context = CreateContext(out Category gay);
			var provider = context.Render<MudDialogProvider>();

			Task<CategorySelection> result = await ShowDialog(context, provider);
			ClickListItem(provider, "Gay");

			CategorySelection selection = await result.WaitAsync(TIMEOUT);
			Assert.IsNotNull(selection);
			Assert.AreSame(gay, selection.Category);
		}

		[TestMethod]
		public async Task AllClicked_ReturnsNullCategory()
		{
			await using var context = CreateContext(out _);
			var provider = context.Render<MudDialogProvider>();

			Task<CategorySelection> result = await ShowDialog(context, provider);
			ClickListItem(provider, MainPageConstants.MAINPAGE_CATEGORY_ALL);

			CategorySelection selection = await result.WaitAsync(TIMEOUT);
			Assert.IsNotNull(selection);
			Assert.IsNull(selection.Category);
		}

		[TestMethod]
		public async Task CloseClicked_ReturnsNull()
		{
			await using var context = CreateContext(out _);
			var provider = context.Render<MudDialogProvider>();

			Task<CategorySelection> result = await ShowDialog(context, provider);
			provider.FindAll("button").Single(button => button.TextContent.Contains(GlobalConstants.CLOSE)).Click();

			Assert.IsNull(await result.WaitAsync(TIMEOUT));
		}

		private static BunitContextUI CreateContext(out Category gay)
		{
			var context = new BunitContextUI();
			gay = new Category() { Title = "Gay", Url = "histoires-erotiques,gay,3.html" };
			context.ServiceCategory.Categories = new List<Category>() { new Category() { Title = "Hétéro" }, gay };

			return context;
		}

		/// <summary>
		/// Opens the dialog and returns its result task, not awaited : it completes when the dialog is closed.
		/// </summary>
		private static async Task<Task<CategorySelection>> ShowDialog(BunitContextUI context, IRenderedComponent<MudDialogProvider> provider)
		{
			var serviceDialog = context.Services.GetRequiredService<IServiceDialog>();
			Task<CategorySelection> result = null;

			// Statement lambda : InvokeAsync must not await the dialog result
			await provider.InvokeAsync(() => { result = serviceDialog.ShowAsync<DialogSelectCategory, CategorySelection>(); });
			provider.WaitForAssertion(() => Assert.AreEqual(3, provider.FindAll(".mud-list-item").Count));

			return result;
		}

		private static void ClickListItem(IRenderedComponent<MudDialogProvider> provider, string text)
		{
			provider.FindAll(".mud-list-item").Single(item => item.TextContent.Contains(text)).Click();
		}
	}
}
