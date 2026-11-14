using Microsoft.AspNetCore.Components;
using XXRead.UI.Services;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// Records the navigations. GoBack changes the location as history.back() does (ServiceDialog waits for it).
	/// </summary>
	public class FakeServiceNavigation : IServiceNavigation
	{
		private readonly NavigationManager _navigationManager;

		public Type CurrentPage { get; private set; }

		public int GoBackCount { get; private set; }

		public FakeServiceNavigation(NavigationManager navigationManager)
		{
			_navigationManager = navigationManager;
		}

		public Task NavigateTo<TComponent>() where TComponent : IComponent
		{
			CurrentPage = typeof(TComponent);

			return Task.CompletedTask;
		}

		public Task GoBack()
		{
			GoBackCount++;
			_navigationManager.NavigateTo(_navigationManager.Uri);

			return Task.CompletedTask;
		}

		public void SetRoot<TComponent>() where TComponent : IComponent
		{
			CurrentPage = typeof(TComponent);
		}
	}
}
