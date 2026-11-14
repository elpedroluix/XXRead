using Microsoft.JSInterop;
using XXRead.UI.Platform;

namespace XXRead.WebHost.Platform
{
	public class ServicePlatformWeb : IServicePlatform
	{
		private readonly IJSRuntime _jsRuntime;

		public ServicePlatformWeb(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}

		public async Task Share(string title, string text, string url)
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync("navigator.share", new { title, text, url });
			}
			catch (JSException)
			{
				// navigator.share is missing on some browsers, or the user closed the share sheet
				await OpenInBrowser(url);
			}
		}

		public async Task OpenInBrowser(string url)
		{
			await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
		}

		public void SetStatusBarColor(string color)
		{
			// No status bar in a browser
		}

		public void SwitchToXamlUI()
		{
			// No XAML UI in a browser
		}

		public void OpenXamlSettings()
		{
			// No XAML UI in a browser
		}
	}
}
