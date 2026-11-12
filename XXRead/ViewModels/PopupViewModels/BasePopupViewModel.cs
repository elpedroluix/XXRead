using CommunityToolkit.Mvvm.Input;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels.PopupViewModels
{
	public class BasePopupViewModel : BaseViewModel
	{
		public RelayCommand ClosePopupCommand { get; set; }

		public BasePopupViewModel(IServiceNavigation serviceNavigation) : base(serviceNavigation)
		{

		}

		public virtual async void ExecuteClosePopupCommand()
		{
			await ServiceNavigation.GoBack();
		}
	}
}
