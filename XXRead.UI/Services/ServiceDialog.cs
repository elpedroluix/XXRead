using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace XXRead.UI.Services
{
	public class ServiceDialog : IServiceDialog
	{
		private readonly IDialogService _dialogService;

		public ServiceDialog(IDialogService dialogService)
		{
			_dialogService = dialogService;
		}

		public async Task<TResult> ShowAsync<TDialog, TResult>(IDictionary<string, object> parameters = null) where TDialog : IComponent
		{
			var dialogParameters = new DialogParameters();
			foreach (var parameter in parameters ?? new Dictionary<string, object>())
			{
				dialogParameters.Add(parameter.Key, parameter.Value);
			}

			IDialogReference dialog = await _dialogService.ShowAsync<TDialog>(string.Empty, dialogParameters);
			DialogResult result = await dialog.Result;

			if (result == null || result.Canceled || result.Data is not TResult data)
			{
				return default;
			}

			return data;
		}

		public async Task ShowMessage(string title, string message)
		{
			await _dialogService.ShowMessageBoxAsync(title, message);
		}
	}
}
