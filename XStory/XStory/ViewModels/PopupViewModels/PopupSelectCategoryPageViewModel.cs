using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XStory.DTO;
using XStory.Helpers;

namespace XStory.ViewModels.PopupViewModels
{
	public class PopupSelectCategoryPageViewModel : BaseViewModel
	{
		BL.SQLite.Contracts.IServiceCategory _serviceCategory;
		private BL.Common.Contracts.IServiceCategory _elServiceCategory;

		#region --- Fields ---
		private Category _selectedCategory;
		private bool _resetCategories;

		private ObservableCollection<DTO.Category> _categories;
		public ObservableCollection<DTO.Category> Categories
		{
			get { return _categories; }
			set { SetProperty(ref _categories, value); }
		}
		#endregion

		public DelegateCommand ClosePopupCommand { get; set; }
		public DelegateCommand<DTO.Category> CategoriesItemTappedCommand { get; set; }
		public DelegateCommand ResetCategoriesCommand { get; set; }

		public PopupSelectCategoryPageViewModel(INavigationService navigationService,
			BL.SQLite.Contracts.IServiceCategory serviceCategory,
			BL.Common.Contracts.IServiceCategory elServiceCategory) : base(navigationService)
		{
			_serviceCategory = serviceCategory;
			_elServiceCategory = elServiceCategory;

			InitCategories();

			ClosePopupCommand = new DelegateCommand(ExecuteClosePopupCommand);
			CategoriesItemTappedCommand = new DelegateCommand<DTO.Category>((category) => ExecuteCategoriesItemTappedCommand(category));
			ResetCategoriesCommand = new DelegateCommand(ExecuteResetCategoriesCommand);
		}

		private async void InitCategories()
		{
			var categories = await _serviceCategory.GetCategories(StaticContext.DATASOURCE, false);
			Categories = new ObservableCollection<Category>(categories.OrderBy(c => c.Title));
		}

		private void ExecuteCategoriesItemTappedCommand(Category category)
		{
			if (category != null)
			{
				_elServiceCategory.SetCurrentCategory(category);
			}

			ClosePopupCommand.Execute();
		}

		private void ExecuteResetCategoriesCommand()
		{
			_elServiceCategory.SetCurrentCategory(null);

			ClosePopupCommand.Execute();
		}

		private async void ExecuteClosePopupCommand()
		{
			await NavigationService.GoBackAsync();
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			try
			{
				List<DTO.Category> categs = parameters.GetValue<List<DTO.Category>>("categories");
				if (categs != null)
				{
					Categories = new ObservableCollection<DTO.Category>(categs);
				}
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
				Categories = null;
			}
		}
	}
}
