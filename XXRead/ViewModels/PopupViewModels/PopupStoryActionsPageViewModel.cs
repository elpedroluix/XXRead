using CommunityToolkit.Mvvm.Input;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.Logger;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels.PopupViewModels
{
	public class PopupStoryActionsPageViewModel : Common.BaseStoryViewModel
	{
		#region --- Fields ---
		IServiceStory _serviceStory;
		#endregion

		#region --- Commands ---
		public RelayCommand ClosePopupCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public PopupStoryActionsPageViewModel(IServiceNavigation serviceNavigation,
			IServiceStory serviceStory) : base(serviceNavigation, serviceStory)
		{
			_serviceStory = serviceStory;

			ClosePopupCommand = new RelayCommand(ExecuteClosePopupCommand);

			this.InitStory();
		}
		#endregion

		private void InitStory()
		{
			Story = _serviceStory.GetCurrentStory();
		}

		private async void ExecuteClosePopupCommand()
		{
			await ServiceNavigation.GoBack();
		}
	}
}
