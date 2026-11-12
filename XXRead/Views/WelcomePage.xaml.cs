namespace XXRead.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WelcomePage : ContentPage
	{
		public WelcomePage(ViewModels.WelcomePageViewModel vm)
		{
			InitializeComponent();

			this.BindingContext = vm;
		}
	}
}