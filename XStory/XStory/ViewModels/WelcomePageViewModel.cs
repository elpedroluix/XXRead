using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using XStory.Helpers;
using XStory.Helpers.Constants;

namespace XStory.ViewModels
{
    public class WelcomePageViewModel : BaseViewModel
    {
        public DelegateCommand<string> DataSourceSelectCommand { get; set; }

        private Dictionary<string, string> _dataSourceDictionary = new Dictionary<string, string>()
        {
            { "XStory" , GlobalConstants.STORIES_SOURCE_XSTORY },
            { "Histoires-de-sexe" , GlobalConstants.STORIES_SOURCE_HDS }
        };

        public WelcomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Helpers.Constants.WelcomePageConstants.WELCOMEPAGE_TITLE;

            DataSourceSelectCommand = new DelegateCommand<string>((param) => ExecuteDataSourceSelectCommand(param));
        }

        private void ExecuteDataSourceSelectCommand(string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new Exception();
            }
            else if (_dataSourceDictionary.ContainsKey(param))
            {
                StaticContext.DATASOURCE = _dataSourceDictionary[param];
                AppSettings.StoriesSource = StaticContext.DATASOURCE;
            }
        }
    }
}
