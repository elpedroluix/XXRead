using XXRead.UI.Platform;

namespace XXRead.Tests.UI.Fakes
{
	public class FakeServicePlatform : IServicePlatform
	{
		public Task Share(string title, string text, string url) => Task.CompletedTask;
		public Task OpenInBrowser(string url) => Task.CompletedTask;
		public void SetStatusBarColor(string color) { }
		public void SwitchToXamlUI() { }
		public void OpenXamlSettings() { }
	}
}
