using System.Reflection;
using Prism.Navigation.Xaml;

namespace XXRead.Helpers
{
	/// <summary>
	/// Prism 9.0.537 cancels every modal pop it did not start, then asks the modal for its INavigationService.</br>
	/// CommunityToolkit's popups (v2) are modal pages unknown to Prism : closing one throws a NullReferenceException.</br>
	/// Fixed in Prism's master (modals not created by Prism are left to MAUI) but not released yet : remove this once Prism is updated.
	/// </summary>
	public static class PrismModalPoppingFix
	{
		public static void Apply(Window window)
		{
			if (window is not Prism.Navigation.PrismWindow)
				return;

			MethodInfo? prismHandlerMethod = typeof(Prism.Navigation.PrismWindow)
				.GetMethod("PrismWindow_ModalPopping", BindingFlags.Instance | BindingFlags.NonPublic);

			// Prism has changed : the fix is probably no longer needed
			if (prismHandlerMethod == null)
				return;

			var prismHandler = (EventHandler<ModalPoppingEventArgs>)Delegate.CreateDelegate(typeof(EventHandler<ModalPoppingEventArgs>), window, prismHandlerMethod);

			window.ModalPopping -= prismHandler;
			window.ModalPopping += (sender, e) =>
			{
				// Modal not created by Prism (e.g. a popup) : let MAUI pop it
				if (e.Modal.GetContainerProvider() == null)
					return;

				prismHandler(sender, e);
			};
		}
	}
}
