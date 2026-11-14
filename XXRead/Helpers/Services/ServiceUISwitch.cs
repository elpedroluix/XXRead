namespace XXRead.Helpers.Services
{
	/// <summary>
	/// Switch between the XAML UI and the Blazor UI (<see cref="BlazorHostPage"/>), stored in <see cref="AppSettings.UseBlazorUI"/>.
	/// Temporary : removed with the XAML UI (step 8.7).
	/// </summary>
	public class ServiceUISwitch
	{
		private readonly IServiceProvider _serviceProvider;

		public ServiceUISwitch(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public Page CreateStartPage()
		{
			return AppSettings.UseBlazorUI
				? _serviceProvider.GetRequiredService<BlazorHostPage>()
				: CreateXamlStartPage();
		}

		public void ShowBlazorUI()
		{
			AppSettings.UseBlazorUI = true;
			SetWindowPage(_serviceProvider.GetRequiredService<BlazorHostPage>());
		}

		public void ShowXamlUI()
		{
			AppSettings.UseBlazorUI = false;
			SetWindowPage(CreateXamlStartPage());
		}

		/// <summary>
		/// XAML UI opened on its settings page (back goes to the XAML MainPage), until the Blazor settings are ported (step 8.5).
		/// </summary>
		public void ShowXamlSettings()
		{
			AppSettings.UseBlazorUI = false;

			// Deferred : called from a click handler of the BlazorWebView being replaced
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				var navigationPage = new Views.MyNavigationPage(_serviceProvider.GetRequiredService<Views.MainPage>());
				Application.Current.Windows[0].Page = navigationPage;
				await navigationPage.PushAsync(_serviceProvider.GetRequiredService<Views.SettingsPage>());
			});
		}

		private Page CreateXamlStartPage()
		{
			Page startPage = AppSettings.FirstRun
				? _serviceProvider.GetRequiredService<Views.WelcomePage>()
				: _serviceProvider.GetRequiredService<Views.MainPage>();

			return new Views.MyNavigationPage(startPage);
		}

		private static void SetWindowPage(Page page)
		{
			// Deferred : the switch is called from a click handler of the page being replaced (toolbar item, BlazorWebView)
			MainThread.BeginInvokeOnMainThread(() => Application.Current.Windows[0].Page = page);
		}
	}
}
