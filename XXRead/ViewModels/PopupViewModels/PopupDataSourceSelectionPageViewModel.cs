using XXRead.Helpers;
using XStory.Logger;
using XXRead.Helpers.Services;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;

namespace XXRead.ViewModels.PopupViewModels
{
	public class PopupDataSourceSelectionPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private XStory.BL.Common.Contracts.IServiceConfig _serviceConfig;
		private XStory.BL.Common.Contracts.IServiceCategory _serviceCategory;

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
		public PopupDataSourceSelectionPageViewModel(INavigationService navigationService,
			XStory.BL.Common.Contracts.IServiceConfig serviceConfig,
			XStory.BL.Common.Contracts.IServiceCategory serviceCategory) : base(navigationService)
		{
			_serviceConfig = serviceConfig;
			_serviceCategory = serviceCategory;

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
				this.SetCurrentDataSource(dataSourceItem);


				ClosePopupCommand.Execute(null);
			}
		}

		private void SetCurrentDataSource(DataSourceItem dataSourceItem)
		{
			_serviceConfig.SetCurrentDataSource(
					(XStory.DTO.Config.DataSources)Enum.Parse(typeof(XStory.DTO.Config.DataSources), dataSourceItem.Name));

			Helpers.StaticContext.DATASOURCE = dataSourceItem.Name;

			_serviceCategory.SetCurrentCategory(null);
		}

		private void ExecuteClosePopupCommand()
		{
			CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default.Send<Helpers.Messaging.ClosePopupMessage, string>(
				new Helpers.Messaging.ClosePopupMessage(0), "ClosePopup");
		}

		public void BuildDataSourceItems(List<DataSourceItem> dataSourceFullList)
		{
			var dataSourceToDisplay = dataSourceFullList.Where(dsfl =>
			dsfl.Name.ToLower() != _serviceConfig.GetCurrentDataSource().ToString().ToLower()).ToList();

			DataSourceItems = dataSourceToDisplay;

			CurrentDataSource = dataSourceFullList.FirstOrDefault(dsi =>
			dsi.Name.ToLower() == _serviceConfig.GetCurrentDataSource().ToString().ToLower());
		}


		/*public override void OnNavigatedTo(INavigationParameters parameters)
		{
			try
			{
				_dataSourceFullList = parameters.GetValue<List<DataSourceItem>>("dataSources");

				this.BuildDataSourceItems(_dataSourceFullList);
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
			}
		}*/
	}
}
