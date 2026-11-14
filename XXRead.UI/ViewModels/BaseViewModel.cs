using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XXRead.UI.Services;

namespace XXRead.UI.ViewModels
{
	public class BaseViewModel : ObservableObject
	{
		protected IServiceNavigation ServiceNavigation { get; private set; }

		public RelayCommand TryAgainCommand { get; set; }

		private string _title;
		private ViewStateEnum _viewState;

		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		public ViewStateEnum ViewState
		{
			get { return _viewState; }
			set { SetProperty(ref _viewState, value); }
		}

		public BaseViewModel(IServiceNavigation serviceNavigation)
		{
			ServiceNavigation = serviceNavigation;

			ViewState = ViewStateEnum.Display;
		}

		/// <summary>
		/// Called by the page each time it is displayed (OnInitializedAsync).
		/// Replaces the Appearing command of the XAML UI.
		/// </summary>
		public virtual Task OnAppearing()
		{
			return Task.CompletedTask;
		}

		protected virtual void ExecuteTryAgainCommand() { }
	}
}
