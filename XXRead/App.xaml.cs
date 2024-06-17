namespace XXRead
{
	public partial class App : Application
	{
		public App()
		//public App(IServiceProvider serviceProvider)
		{
			// ↓↓↓ Pour récup une instance de view model depuis le container de dependency injection
			// var x = serviceProvider.GetService<ViewModels.MainPageViewModel>();

			InitializeComponent();

			MainPage = new AppShell();
		}
	}
}
