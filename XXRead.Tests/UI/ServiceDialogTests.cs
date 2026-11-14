using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using XXRead.Core.Models;
using XXRead.Tests.UI.Fakes;
using XXRead.UI.Dialogs;
using XXRead.UI.Services;

namespace XXRead.Tests.UI
{
	/// <summary>
	/// Back button with a dialog open : the dialog closes, the page stays.
	/// </summary>
	[TestClass]
	public class ServiceDialogTests
	{
		private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(5);

		[TestMethod]
		public async Task BackWhileDialogOpen_ClosesDialogWithoutLeavingPage()
		{
			await using var context = new BunitContextUI();
			var provider = context.Render<MudDialogProvider>();
			var navigationManager = context.Services.GetRequiredService<NavigationManager>();

			Task<Story> result = await ShowDialog(context, provider);
			// What the back button does : the location changes
			await provider.InvokeAsync(() => navigationManager.NavigateTo(navigationManager.Uri));

			Assert.IsNull(await result.WaitAsync(TIMEOUT));
			provider.WaitForAssertion(() => Assert.AreEqual(0, provider.FindAll(".mud-list-item").Count));
			Assert.AreEqual(0, context.ServiceNavigation.GoBackCount);
		}

		[TestMethod]
		public async Task DialogClosed_RemovesItsHistoryEntry()
		{
			await using var context = new BunitContextUI();
			var provider = context.Render<MudDialogProvider>();

			Task<Story> result = await ShowDialog(context, provider);
			provider.FindAll(".mud-list-item")[0].Click();

			Assert.AreEqual("Chapitre 1", (await result.WaitAsync(TIMEOUT)).Title);
			Assert.AreEqual(1, context.ServiceNavigation.GoBackCount);
		}

		/// <summary>
		/// Opens the chapters dialog and returns its result task, not awaited : it completes when the dialog is closed.
		/// </summary>
		private static async Task<Task<Story>> ShowDialog(BunitContextUI context, IRenderedComponent<MudDialogProvider> provider)
		{
			var serviceDialog = context.Services.GetRequiredService<IServiceDialog>();
			var story = new Story() { Title = "Story" };
			story.ChaptersList.Add(new Story() { Title = "Chapitre 1" });
			story.ChaptersList.Add(new Story() { Title = "Chapitre 2" });
			var parameters = new Dictionary<string, object>() { { nameof(DialogChapters.Story), story } };
			Task<Story> result = null;

			// Statement lambda : InvokeAsync must not await the dialog result
			await provider.InvokeAsync(() => { result = serviceDialog.ShowAsync<DialogChapters, Story>(parameters); });
			provider.WaitForAssertion(() => Assert.AreEqual(2, provider.FindAll(".mud-list-item").Count));

			return result;
		}
	}
}
