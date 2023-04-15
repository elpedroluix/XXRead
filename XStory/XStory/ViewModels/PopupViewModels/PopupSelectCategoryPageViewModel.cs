using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XStory.DTO;

namespace XStory.ViewModels.PopupViewModels
{
    public class PopupSelectCategoryPageViewModel : BaseViewModel
    {
        BL.SQLite.Contracts.IServiceCategory _serviceCategory;

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

        public PopupSelectCategoryPageViewModel(INavigationService navigationService, BL.SQLite.Contracts.IServiceCategory serviceCategory) : base(navigationService)
        {
            _serviceCategory = serviceCategory;

            InitCategories();

            ClosePopupCommand = new DelegateCommand(ExecuteClosePopupCommand);
            CategoriesItemTappedCommand = new DelegateCommand<DTO.Category>((category) => ExecuteCategoriesItemTappedCommand(category));
            ResetCategoriesCommand = new DelegateCommand(ExecuteResetCategoriesCommand);
        }

        private async void InitCategories()
        {
            var categories = await _serviceCategory.GetCategories(false);
            Categories = new ObservableCollection<Category>(categories.OrderBy(c => c.Title));
        }

        private void ExecuteCategoriesItemTappedCommand(Category category)
        {
            if (category != null)
            {
                _selectedCategory = category;
            }

            ClosePopupCommand.Execute();
        }

        private void ExecuteResetCategoriesCommand()
        {
            _resetCategories = true;
            ClosePopupCommand.Execute();
        }

        private async void ExecuteClosePopupCommand()
        {
            NavigationParameters navigationParameters = new NavigationParameters()
            {
                { "selectedCategory", _selectedCategory }
            };
            if (_resetCategories)
            {
                navigationParameters.Add("resetCategories", _resetCategories);
            }

            await NavigationService.GoBackAsync(navigationParameters);
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
