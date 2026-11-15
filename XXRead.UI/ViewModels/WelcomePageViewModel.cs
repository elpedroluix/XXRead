using CommunityToolkit.Mvvm.Input;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.UI.Constants;
using XXRead.UI.Platform;
using XXRead.UI.Services;

namespace XXRead.UI.ViewModels
{
	/// <summary>
	/// Copied from the XAML UI's WelcomePageViewModel.
	/// </summary>
	public class WelcomePageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private IServiceConfig _serviceConfig;
		private IServiceAppSettings _serviceAppSettings;

		/// <summary>
		/// Data sources offered on the first run, with the text of their button. Demo is not offered.
		/// </summary>
		public IReadOnlyDictionary<string, DataSources> DataSourceChoices { get; } = new Dictionary<string, DataSources>()
		{
			{ WelcomePageConstants.WELCOMEPAGE_DATASOURCE_XSTORY, DataSources.XStory },
			{ WelcomePageConstants.WELCOMEPAGE_DATASOURCE_HDS, DataSources.HDS }
		};
		#endregion

		#region --- Commands ---
		public RelayCommand<DataSources> DataSourceSelectCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public WelcomePageViewModel(IServiceNavigation serviceNavigation,
			IServiceAppSettings serviceAppSettings,
			IServiceConfig serviceConfig) : base(serviceNavigation)
		{
			_serviceAppSettings = serviceAppSettings;
			_serviceConfig = serviceConfig;

			Title = WelcomePageConstants.WELCOMEPAGE_TITLE;

			DataSourceSelectCommand = new RelayCommand<DataSources>(ExecuteDataSourceSelectCommand);
		}
		#endregion

		private void ExecuteDataSourceSelectCommand(DataSources dataSource)
		{
			_serviceConfig.SetCurrentDataSource(dataSource);
			_serviceAppSettings.FirstRun = false;

			ServiceNavigation.SetRoot<Pages.MainPage>();
		}
	}
}
