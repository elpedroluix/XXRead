using Microsoft.AspNetCore.Components;

namespace XXRead.UI.Services
{
	/// <summary>
	/// Dialogs for the ViewModels, without depending on MudBlazor (testable with a fake).
	/// </summary>
	public interface IServiceDialog
	{
		/// <summary>
		/// Shows the dialog <typeparamref name="TDialog"/> and waits for it to close.
		/// Returns default when the dialog is canceled (back button, click outside, cancel button).
		/// </summary>
		Task<TResult> ShowAsync<TDialog, TResult>(IDictionary<string, object> parameters = null) where TDialog : IComponent;

		Task ShowMessage(string title, string message);
	}
}
