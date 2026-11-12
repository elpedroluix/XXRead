namespace XXRead.Views.Settings
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsAppearancePage : ContentPage
	{
		public SettingsAppearancePage(ViewModels.Settings.SettingsAppearancePageViewModel vm)
		{
			InitializeComponent();

			this.BindingContext = vm;
		}
	}
}