namespace XXRead.Helpers.Services
{
	/// <summary>
	/// Page navigation (push / pop) inside the window's <see cref="Views.MyNavigationPage"/>.
	/// </summary>
	public class ServiceNavigation : IServiceNavigation
	{
		private readonly IServiceProvider _serviceProvider;

		public ServiceNavigation(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task NavigateTo<TPage>() where TPage : Page
		{
			// Pages are registered in MauiProgram.RegisterViews : their ViewModel is injected in the page's ctor
			TPage page = _serviceProvider.GetRequiredService<TPage>();

			await GetNavigation().PushAsync(page);
		}

		public async Task GoBack()
		{
			INavigation navigation = GetNavigation();

			if (navigation.ModalStack.Count > 0)
			{
				await navigation.PopModalAsync();
			}
			else if (navigation.NavigationStack.Count > 1)
			{
				await navigation.PopAsync();
			}
		}

		public void SetRoot<TPage>() where TPage : Page
		{
			TPage page = _serviceProvider.GetRequiredService<TPage>();

			Application.Current.Windows[0].Page = new Views.MyNavigationPage(page);
		}

		private static INavigation GetNavigation()
		{
			return Application.Current.Windows[0].Page.Navigation;
		}
	}
}
