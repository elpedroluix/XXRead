using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XStory.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {

        public DelegateCommand CacaCommand { get; set; }

        public SettingsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Helpers.Constants.SettingsPageConstants.SETTINGS_TITLE;

            CacaCommand = new DelegateCommand(ExecuteCacaCommand);
        }

        private void ExecuteCacaCommand()
        {
            Title = "lolol";
        }
    }
}
