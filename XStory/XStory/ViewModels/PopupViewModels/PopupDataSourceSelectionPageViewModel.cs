using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using XStory.Helpers;
using XStory.Logger;

namespace XStory.ViewModels.PopupViewModels
{
	public class PopupDataSourceSelectionPageViewModel : BaseViewModel
	{
		#region --- Fields ---
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

		public DelegateCommand ClosePopupCommand { get; set; }
		public DelegateCommand<DataSourceItem> DataSourceItemTappedCommand { get; set; }
		#endregion

		public PopupDataSourceSelectionPageViewModel(INavigationService navigationService) : base(navigationService)
		{
			ClosePopupCommand = new DelegateCommand(ExecuteClosePopupCommand);
			DataSourceItemTappedCommand = new DelegateCommand<DataSourceItem>((dataSourceItem) => ExecuteDataSourceItemTappedCommand(dataSourceItem));
		}

		private void ExecuteDataSourceItemTappedCommand(DataSourceItem dataSourceItem)
		{
			if (dataSourceItem != null
				&& !string.IsNullOrEmpty(dataSourceItem.Name)
				&& dataSourceItem.Name != StaticContext.DATASOURCE)
			{
				CurrentDataSource = dataSourceItem;
				StaticContext.DATASOURCE = dataSourceItem.Name;

				ClosePopupCommand.Execute();
			}
		}

		private async void ExecuteClosePopupCommand()
		{
			await NavigationService.GoBackAsync();
		}

		private void BuildDataSourceItems(List<DataSourceItem> dataSourceFullList)
		{
			var dataSourceToDisplay = dataSourceFullList.Where(dsfl => dsfl.Name != StaticContext.DATASOURCE).ToList();

			DataSourceItems = dataSourceToDisplay;

			CurrentDataSource = dataSourceFullList.FirstOrDefault(dsi => dsi.Name == StaticContext.DATASOURCE);
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			try
			{
				_dataSourceFullList = parameters.GetValue<List<DataSourceItem>>("dataSources");

				BuildDataSourceItems(_dataSourceFullList);
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
			}
		}
	}
}
