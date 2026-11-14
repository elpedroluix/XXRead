using XXRead.Helpers;
using XStory.Logger;
using XXRead.Helpers.Services;
using CommunityToolkit.Mvvm.Input;

namespace XXRead.ViewModels.PopupViewModels
{
    public class PopupDataSourceSelectionPageViewModel : BasePopupViewModel
    {
        #region --- Fields ---
        private XXRead.Core.Services.IServiceConfig _serviceConfig;
        private XXRead.Core.Services.IServiceCategory _serviceCategory;
        private IServicePopup _servicePopup;

        private DataSourceItem _currentDataSource;
        public DataSourceItem CurrentDataSource
        {
            get { return _currentDataSource; }
            set { SetProperty(ref _currentDataSource, value); }
        }

        private List<DataSourceItem> _dataSourceFullList;

        private List<DataSourceItem> _dataSourceItems;
        public List<DataSourceItem> DataSourceItems
        {
            get { return _dataSourceItems; }
            set { SetProperty(ref _dataSourceItems, value); }
        }

        public RelayCommand ClosePopupCommand { get; set; }
        public RelayCommand<DataSourceItem> DataSourceItemTappedCommand { get; set; }
        #endregion

        #region --- Ctor ---
        public PopupDataSourceSelectionPageViewModel(IServiceNavigation serviceNavigation,
            XXRead.Core.Services.IServiceConfig serviceConfig,
            XXRead.Core.Services.IServiceCategory serviceCategory,
            IServicePopup servicePopup) : base(serviceNavigation)
        {
            _serviceConfig = serviceConfig;
            _serviceCategory = serviceCategory;
            _servicePopup = servicePopup;

            ClosePopupCommand = new RelayCommand(ExecuteClosePopupCommand);
            DataSourceItemTappedCommand = new RelayCommand<DataSourceItem>((dataSourceItem) => ExecuteDataSourceItemTappedCommand(dataSourceItem));
        }
        #endregion

        private void ExecuteDataSourceItemTappedCommand(DataSourceItem dataSourceItem)
        {
            if (dataSourceItem != null
                && !string.IsNullOrEmpty(dataSourceItem.Name)
                && dataSourceItem.Name != _serviceConfig.GetCurrentDataSource().ToString().ToLower())
            {
                CurrentDataSource = dataSourceItem;


                ClosePopupCommand.Execute(null);
            }
        }

        public override void ExecuteClosePopupCommand()
        {
            RequestClose(CurrentDataSource);
        }

        public void BuildDataSourceItems(List<DataSourceItem> dataSourceFullList)
        {
            var dataSourceToDisplay = dataSourceFullList.Where(dsfl =>
            dsfl.Name.ToLower() != _serviceConfig.GetCurrentDataSource().ToString().ToLower()).ToList();

            DataSourceItems = dataSourceToDisplay;

            CurrentDataSource = dataSourceFullList.FirstOrDefault(dsi =>
            dsi.Name.ToLower() == _serviceConfig.GetCurrentDataSource().ToString().ToLower());
        }

        private async void RequestClose(DataSourceItem dsItem = null)
        {
            await _servicePopup.ClosePopupAsync(dsItem);
        }
    }
}
