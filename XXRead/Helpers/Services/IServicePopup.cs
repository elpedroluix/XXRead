using CommunityToolkit.Maui.Core;

namespace XXRead.Helpers.Services
{
	public interface IServicePopup
	{
		/// <summary>
		/// Displays the popup <typeparamref name="TPopup"/> and waits for its result.</br>
		/// <paramref name="onPresenting"/> lets the caller initialize the popup's ViewModel before display.
		/// </summary>
		Task<IPopupResult<TResult>> ShowPopupAsync<TPopup, TViewModel, TResult>(Action<TViewModel>? onPresenting = null)
			where TPopup : View;

		/// <summary>
		/// Closes the displayed popup, returning <paramref name="result"/> to the caller of <see cref="ShowPopupAsync"/>.
		/// </summary>
		Task ClosePopupAsync<TResult>(TResult result);
	}
}
