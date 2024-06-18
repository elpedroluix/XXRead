
using Microsoft.Extensions.Primitives;

namespace XXRead.Views.Popup
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PopupDataSourceSelectionPage : CommunityToolkit.Maui.Views.Popup
	{
		public PopupDataSourceSelectionPage(ViewModels.PopupViewModels.PopupDataSourceSelectionPageViewModel viewModel)
		{
			InitializeComponent();

			this.BindingContext = viewModel;

			this.RegisterCloseMessage();
		}

		private void RegisterCloseMessage()
		{
			CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default
				.Register<PopupDataSourceSelectionPage, Helpers.Messaging.ClosePopupMessage, string>(
				this,
				"ClosePopup",
				async (recipient, message) =>
				{
					await recipient.Dispatcher.DispatchAsync(
						async () =>
						{
							if (message.Value == 0)
							{

								await recipient.CloseAsync();
							}
						});
					CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default
								.Unregister<Helpers.Messaging.ClosePopupMessage, string>(recipient, "ClosePopup");
				});

		}

		//private void Close()
		//{
		//	this.CloseAsync();
		//}
	}
}