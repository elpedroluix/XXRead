using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace XXRead.UI.Services
{
	public class ServiceDialog : IServiceDialog
	{
		private static readonly TimeSpan NAVIGATION_TIMEOUT = TimeSpan.FromSeconds(2);

		/// <summary>
		/// By default MudDialogProvider closes a dialog only when the path changes : the back button keeps the same path.
		/// The other options come from MudDialogProvider.
		/// </summary>
		private static readonly DialogOptions _dialogOptions = new DialogOptions() { CloseOnNavigation = true };

		private readonly IDialogService _dialogService;
		private readonly NavigationManager _navigationManager;
		private readonly IServiceNavigation _serviceNavigation;

		public ServiceDialog(IDialogService dialogService,
			NavigationManager navigationManager,
			IServiceNavigation serviceNavigation)
		{
			_dialogService = dialogService;
			_navigationManager = navigationManager;
			_serviceNavigation = serviceNavigation;
		}

		public async Task<TResult> ShowAsync<TDialog, TResult>(IDictionary<string, object> parameters = null) where TDialog : IComponent
		{
			var dialogParameters = new DialogParameters();
			foreach (var parameter in parameters ?? new Dictionary<string, object>())
			{
				dialogParameters.Add(parameter.Key, parameter.Value);
			}

			DialogResult result = null;
			await ShowWithBackEntry(async () =>
			{
				IDialogReference dialog = await _dialogService.ShowAsync<TDialog>(string.Empty, dialogParameters, _dialogOptions);
				result = await dialog.Result;
			});

			if (result == null || result.Canceled || result.Data is not TResult data)
			{
				return default;
			}

			return data;
		}

		public async Task ShowMessage(string title, string message)
		{
			await ShowWithBackEntry(() => _dialogService.ShowMessageBoxAsync(title, message, options: _dialogOptions));
		}

		/// <summary>
		/// The back button (Android, browser) closes the dialog instead of leaving the page :
		/// the dialog gets its own history entry, and MudDialogProvider closes the dialog when the location changes (CloseOnNavigation).
		/// When the dialog is closed another way, its history entry is removed.
		/// </summary>
		private async Task ShowWithBackEntry(Func<Task> showDialog)
		{
			// Same url : new history entry, the page stays where it is
			await WaitForLocationChanged(() =>
			{
				_navigationManager.NavigateTo(_navigationManager.Uri);
				return Task.CompletedTask;
			});

			bool closedByBack = false;
			EventHandler<LocationChangedEventArgs> onLocationChanged = (sender, e) => closedByBack = true;

			_navigationManager.LocationChanged += onLocationChanged;
			try
			{
				await showDialog();

				// Closed by the back button : the dialog result is set by MudDialogProvider's LocationChanged handler,
				// this code may resume before onLocationChanged is called. Yield lets the remaining handlers run.
				await Task.Yield();
			}
			finally
			{
				_navigationManager.LocationChanged -= onLocationChanged;
			}

			if (closedByBack) return;

			await WaitForLocationChanged(_serviceNavigation.GoBack);
		}

		/// <summary>
		/// Navigates and waits for the new location (the WebView notifies it asynchronously).
		/// Gives up after <see cref="NAVIGATION_TIMEOUT"/> rather than blocking the dialog.
		/// </summary>
		private async Task WaitForLocationChanged(Func<Task> navigate)
		{
			// Asynchronous continuation : the other LocationChanged handlers (MudDialogProvider) run first
			var locationChanged = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
			EventHandler<LocationChangedEventArgs> onLocationChanged = (sender, e) => locationChanged.TrySetResult();

			_navigationManager.LocationChanged += onLocationChanged;
			try
			{
				await navigate();
				await locationChanged.Task.WaitAsync(NAVIGATION_TIMEOUT);
			}
			catch (TimeoutException ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
			}
			finally
			{
				_navigationManager.LocationChanged -= onLocationChanged;
			}
		}
	}
}
