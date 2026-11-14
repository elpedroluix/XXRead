using CommunityToolkit.Mvvm.Input;
using XXRead.Logger;
using XXRead.UI.Constants;
using XXRead.UI.Services;

namespace XXRead.UI.ViewModels
{
	/// <summary>
	/// Logs page (the XAML UI's LogsView, which was never displayed). Transient : the logs are read again each time.
	/// </summary>
	public class LogsPageViewModel : BaseViewModel
	{
		private IServiceLogs _serviceLogs;

		private List<Log> _logs;
		/// <summary>
		/// Most recent first.
		/// </summary>
		public List<Log> Logs
		{
			get { return _logs; }
			set { SetProperty(ref _logs, value); }
		}

		public LogsPageViewModel(IServiceNavigation serviceNavigation, IServiceLogs serviceLogs) : base(serviceNavigation)
		{
			_serviceLogs = serviceLogs;

			Title = SettingsPageConstants.SETTINGS_LOGS_PAGE_TITLE;
			ViewState = ViewStateEnum.Loading;

			TryAgainCommand = new RelayCommand(ExecuteTryAgainCommand);
		}

		public override async Task OnAppearing()
		{
			await InitLogs();
		}

		private async Task InitLogs()
		{
			ViewState = ViewStateEnum.Loading;

			List<Log> logs = await _serviceLogs.GetLogs();
			if (logs == null)
			{
				ViewState = ViewStateEnum.Error;
				return;
			}

			// "yyyy-MM-dd HH:mm:ss" : the text order is the chronological order (no DateTime.Parse, which fails on a malformed date)
			Logs = logs.OrderByDescending(log => log.Date, StringComparer.Ordinal).ToList();
			ViewState = ViewStateEnum.Display;
		}

		protected override async void ExecuteTryAgainCommand()
		{
			await InitLogs();
		}
	}
}
