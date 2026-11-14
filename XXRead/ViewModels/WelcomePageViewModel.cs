using CommunityToolkit.Mvvm.Input;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels
{
    public class WelcomePageViewModel : BaseViewModel
    {
        private IServiceConfig _serviceConfig;

        public RelayCommand<string> DataSourceSelectCommand { get; set; }

        private Dictionary<string, DataSources> _dataSourceDictionary = new Dictionary<string, DataSources>()
        {
            { "XStory" , DataSources.XStory },
            { "Histoires-de-sexe" , DataSources.HDS }
        };

        public WelcomePageViewModel(IServiceNavigation serviceNavigation,
            IServiceConfig serviceConfig) : base(serviceNavigation)
        {
            Title = Helpers.Constants.WelcomePageConstants.WELCOMEPAGE_TITLE;

            _serviceConfig = serviceConfig;

            DataSourceSelectCommand = new RelayCommand<string>((param) => ExecuteDataSourceSelectCommand(param));
        }

        private void ExecuteDataSourceSelectCommand(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new Exception();
            }
            else if (_dataSourceDictionary.ContainsKey(param))
            {
                _serviceConfig.SetCurrentDataSource(_dataSourceDictionary[param]);

                ServiceNavigation.SetRoot<Views.MainPage>();
            }
        }
    }
}
