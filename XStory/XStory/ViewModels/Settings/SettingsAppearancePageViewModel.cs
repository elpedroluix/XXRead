using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using XStory.Helpers.Constants;

namespace XStory.ViewModels.Settings
{
    public class SettingsAppearancePageViewModel : BaseViewModel
    {
        public SettingsAppearancePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = SettingsPageConstants.SETTINGS_APPEARANCE_PAGE_TITLE;
        }
    }
}
