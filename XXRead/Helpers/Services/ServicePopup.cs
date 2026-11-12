using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;

namespace XXRead.Helpers.Services
{
	/// <summary>
	/// Wraps CommunityToolkit's popups (v2), which no longer allow initializing the ViewModel before display.
	/// </summary>
	public class ServicePopup : IServicePopup
	{
		private readonly IServiceProvider _serviceProvider;

		public ServicePopup(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task<IPopupResult<TResult>> ShowPopupAsync<TPopup, TViewModel, TResult>(Action<TViewModel>? onPresenting = null)
			where TPopup : View
		{
			// Popup and ViewModel are registered with AddTransientPopup : the ViewModel is injected in the popup's ctor
			TPopup popup = _serviceProvider.GetRequiredService<TPopup>();

			onPresenting?.Invoke((TViewModel)popup.BindingContext);

			return await GetCurrentPage().ShowPopupAsync<TResult>(popup);
		}

		public async Task ClosePopupAsync<TResult>(TResult result)
		{
			try
			{
				await GetCurrentPage().ClosePopupAsync(result);
			}
			catch (Exception ex)
			{
				// e.g. PopupNotFoundException when the close is requested twice (double tap)
				XStory.Logger.ServiceLog.Error(ex);
			}
		}

		private static Page GetCurrentPage()
		{
			return Application.Current?.Windows[0].Page
				?? throw new InvalidOperationException("No page available to display a popup.");
		}
	}
}
