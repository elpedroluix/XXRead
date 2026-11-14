namespace XXRead.UI.Platform
{
	/// <summary>
	/// Platform features used by the UI, implemented by each host.
	/// </summary>
	public interface IServicePlatform
	{
		Task Share(string title, string text, string url);

		Task OpenInBrowser(string url);

		/// <summary>
		/// Color of the system status bar (Android). Does nothing where there is no status bar.
		/// </summary>
		void SetStatusBarColor(string color);

		/// <summary>
		/// Goes back to the XAML UI. Temporary : removed with the XAML UI (step 8.7).
		/// </summary>
		void SwitchToXamlUI();
	}
}
