using CommunityToolkit.Mvvm.Input;
using XStory.DTO;
using XStory.Logger;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels.PopupViewModels
{
	public class PopupStoryActionsPageViewModel : Common.BaseStoryViewModel
	{
		#region --- Fields ---
		XXRead.Core.Services.IServiceStory _serviceStory;
		#endregion

		#region --- Commands ---
		public RelayCommand ClosePopupCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public PopupStoryActionsPageViewModel(IServiceNavigation serviceNavigation,
			XXRead.Core.Services.IServiceStory serviceStory) : base(serviceNavigation, serviceStory)
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
