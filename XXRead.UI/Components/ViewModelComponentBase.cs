using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace XXRead.UI.Components
{
	/// <summary>
	/// Base of the pages bound to a ViewModel : the page is rendered again each time a ViewModel property changes.
	/// A page that overrides <see cref="OnInitialized"/> or <see cref="Dispose"/> must call the base method.
	/// </summary>
	public abstract class ViewModelComponentBase<TViewModel> : ComponentBase, IDisposable
		where TViewModel : INotifyPropertyChanged
	{
		[Inject]
		protected TViewModel ViewModel { get; set; }

		protected override void OnInitialized()
		{
			ViewModel.PropertyChanged += OnViewModelPropertyChanged;
		}

		public virtual void Dispose()
		{
			ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
		}

		private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// PropertyChanged can be raised from any thread (async commands)
			InvokeAsync(StateHasChanged);
		}
	}
}
