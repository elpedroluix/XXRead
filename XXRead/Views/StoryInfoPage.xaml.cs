namespace XXRead.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StoryInfoPage : ContentPage
	{
		public StoryInfoPage(ViewModels.StoryInfoPageViewModel vm)
		{
			InitializeComponent();

			this.BindingContext = vm;
		}
	}
}