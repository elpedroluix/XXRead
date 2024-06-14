using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XStory.DTO;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels.PopupViewModels
{
	public class PopupSelectCategoryPageViewModel : BasePopupViewModel
	{
		private XStory.BL.Common.Contracts.IServiceCategory _serviceCategory;

		#region --- Fields ---
		private Category _selectedCategory;
		private bool _resetCategories;

		private ObservableCollection<Category> _categories;
		public ObservableCollection<Category> Categories
		{
			get { return _categories; }
			set { SetProperty(ref _categories, value); }
		}
		#endregion

		public RelayCommand<Category> CategoriesItemTappedCommand { get; set; }
		public RelayCommand ResetCategoriesCommand { get; set; }

		public PopupSelectCategoryPageViewModel(INavigationService navigationService,
			XStory.BL.Common.Contracts.IServiceCategory serviceCategory) : base(navigationService)
		{
			_serviceCategory = serviceCategory;

			ClosePopupCommand = new RelayCommand(ExecuteClosePopupCommand);
			CategoriesItemTappedCommand = new RelayCommand<Category>((category) => ExecuteCategoriesItemTappedCommand(category));
			ResetCategoriesCommand = new RelayCommand(ExecuteResetCategoriesCommand);

			this.InitCategories();
		}

		private async void InitCategories()
		{
			Categories = new ObservableCollection<Category>(await _serviceCategory.GetCategoriesDB(false));
		}

		private void ExecuteCategoriesItemTappedCommand(Category category)
		{
			if (category != null)
			{
				_serviceCategory.SetCurrentCategory(category);
			}

			ClosePopupCommand.Execute(null);
		}

		private void ExecuteResetCategoriesCommand()
		{
			_serviceCategory.SetCurrentCategory(null);

			ClosePopupCommand.Execute(null);
		}
	}
}
