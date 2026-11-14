namespace XXRead.Views
{
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage(ViewModels.SettingsPageViewModel vm)
		{
			InitializeComponent();

			this.BindingContext = vm;

#if DEBUG
			// Temporary : switch to the Blazor UI while it is being ported (removed at step 8.7)
			ToolbarItems.Add(new ToolbarItem("Blazor", null, () => IPlatformApplication.Current.Services.GetRequiredService<Helpers.Services.ServiceUISwitch>().ShowBlazorUI()));
#endif
		}
	}
}
