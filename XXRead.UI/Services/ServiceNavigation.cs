using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace XXRead.UI.Services
{
	public class ServiceNavigation : IServiceNavigation
	{
		private readonly NavigationManager _navigationManager;
		private readonly IJSRuntime _jsRuntime;

		public ServiceNavigation(NavigationManager navigationManager, IJSRuntime jsRuntime)
		{
			_navigationManager = navigationManager;
			_jsRuntime = jsRuntime;
		}

		public Task NavigateTo<TComponent>() where TComponent : IComponent
		{
			_navigationManager.NavigateTo(GetRoute<TComponent>());

			return Task.CompletedTask;
		}

		public async Task GoBack()
		{
			await _jsRuntime.InvokeVoidAsync("history.back");
		}

		public void SetRoot<TComponent>() where TComponent : IComponent
		{
			_navigationManager.NavigateTo(GetRoute<TComponent>(), replace: true);
		}

		private static string GetRoute<TComponent>()
		{
			RouteAttribute route = typeof(TComponent).GetCustomAttribute<RouteAttribute>();
			if (route == null)
			{
				throw new InvalidOperationException($"{typeof(TComponent).Name} has no @page directive.");
			}

			return route.Template;
		}
	}
}
