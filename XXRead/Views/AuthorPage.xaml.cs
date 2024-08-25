namespace XXRead.Views
{
	public partial class AuthorPage : ContentPage
	{
		public AuthorPage(ViewModels.AuthorPageViewModel viewModel)
		{
			InitializeComponent();

			this.BindingContext = viewModel;
		}
	}
}