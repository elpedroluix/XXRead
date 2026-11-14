using XXRead.UI.Platform;

namespace XXRead.Helpers.Services
{
	public class ServicePlatformMAUI : IServicePlatform
	{
		private readonly ServiceUISwitch _serviceUISwitch;

		public ServicePlatformMAUI(ServiceUISwitch serviceUISwitch)
		{
			_serviceUISwitch = serviceUISwitch;
		}

		public async Task Share(string title, string text, string url)
		{
			await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(new ShareTextRequest()
			{
				Uri = url,
				Text = text,
				Title = title
			});
		}

		public async Task OpenInBrowser(string url)
		{
			await Browser.Default.OpenAsync(new Uri(url), BrowserLaunchMode.External);
		}

		public void SetStatusBarColor(string color)
		{
#if ANDROID || IOS
			MainThread.BeginInvokeOnMainThread(() =>
				CommunityToolkit.Maui.Core.Platform.StatusBar.SetColor(Color.FromArgb(color)));
#endif
		}

		public void SwitchToXamlUI()
		{
			_serviceUISwitch.ShowXamlUI();
		}
	}
}
