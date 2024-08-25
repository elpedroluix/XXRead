namespace XXRead.Views
{
	public partial class AuthorPage_bak : TabbedPage
	{
		public AuthorPage_bak(ViewModels.AuthorPageViewModel viewModel)
		{
			InitializeComponent();

			this.BindingContext = viewModel;
		}
	}
}
