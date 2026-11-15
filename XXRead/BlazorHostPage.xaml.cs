namespace XXRead
{
	/// <summary>
	/// Hosts the whole Blazor UI (XXRead.UI) in a single BlazorWebView.
	/// </summary>
	public partial class BlazorHostPage : ContentPage
	{
		public BlazorHostPage()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Color painted behind the navigation bar / home handle (edge-to-edge).
		/// </summary>
		public void SetNavigationBarColor(Color color)
		{
			NavigationBarArea.BackgroundColor = color;
		}
	}
}
