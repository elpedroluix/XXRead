using XXRead.UI.Platform;

namespace XXRead.Helpers.Services
{
	public class ServicePlatformMAUI : IServicePlatform
	{
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
			MainThread.BeginInvokeOnMainThread(() =>
			{
				// Edge-to-edge (Android 15+) : the status bar is transparent, the page background (left free by SafeAreaEdges) is painted behind it
				if (Application.Current.Windows[0].Page is BlazorHostPage blazorHostPage)
				{
					blazorHostPage.BackgroundColor = Color.FromArgb(color);
				}

#if ANDROID
				// Before Android 15, the status bar has its own color (ignored from Android 15 on)
				if (!OperatingSystem.IsAndroidVersionAtLeast(35))
				{
					Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor(color));
				}
#endif
			});
		}

		public void SetNavigationBarColor(string color)
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				// Edge-to-edge (Android 15+) : the navigation bar is transparent, the grid left free by SafeAreaEdges is painted behind it
				if (Application.Current.Windows[0].Page is BlazorHostPage blazorHostPage)
				{
					blazorHostPage.SetNavigationBarColor(Color.FromArgb(color));
				}

#if ANDROID
				var window = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window;
				if (window == null) return;

				// Before Android 15, the navigation bar has its own color (ignored from Android 15 on)
				if (!OperatingSystem.IsAndroidVersionAtLeast(35))
				{
					window.SetNavigationBarColor(Android.Graphics.Color.ParseColor(color));
				}

				// Dark buttons / handle on a light background
				new AndroidX.Core.View.WindowInsetsControllerCompat(window, window.DecorView).AppearanceLightNavigationBars = Color.FromArgb(color).GetLuminosity() > 0.5f;
#endif
			});
		}
	}
}
