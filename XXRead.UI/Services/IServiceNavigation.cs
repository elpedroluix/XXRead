using Microsoft.AspNetCore.Components;

namespace XXRead.UI.Services
{
	public interface IServiceNavigation
	{
		/// <summary>
		/// Navigates to the page <typeparamref name="TComponent"/>, found by its <see cref="RouteAttribute"/>.
		/// </summary>
		Task NavigateTo<TComponent>() where TComponent : IComponent;

		/// <summary>
		/// Goes back in the WebView history.
		/// </summary>
		Task GoBack();

		/// <summary>
		/// Navigates to the page <typeparamref name="TComponent"/> and replaces the current history entry (no way back).
		/// </summary>
		void SetRoot<TComponent>() where TComponent : IComponent;
	}
}
