namespace XXRead.Helpers.Services
{
	public interface IServiceNavigation
	{
		/// <summary>
		/// Resolves the page <typeparamref name="TPage"/> from the DI container and pushes it on the navigation stack.
		/// </summary>
		Task NavigateTo<TPage>() where TPage : Page;

		/// <summary>
		/// Closes the current modal page if there is one, otherwise pops the current page from the navigation stack.
		/// </summary>
		Task GoBack();

		/// <summary>
		/// Replaces the whole navigation stack with the page <typeparamref name="TPage"/> (no way back).
		/// </summary>
		void SetRoot<TPage>() where TPage : Page;
	}
}
