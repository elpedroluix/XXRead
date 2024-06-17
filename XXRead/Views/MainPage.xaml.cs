namespace XXRead.Views
{
	public partial class MainPage : ContentPage
	{
		public MainPage(ViewModels.MainPageViewModel vm)
		{
			InitializeComponent();

			this.BindingContext = vm;
		}
	}
}
