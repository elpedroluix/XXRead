﻿using XXRead.Helpers.Constants;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels.Settings
{
	public class SettingsAppearancePageViewModel : BaseViewModel
	{
		public SettingsAppearancePageViewModel(INavigationService navigationService) : base(navigationService)
		{
			Title = SettingsPageConstants.SETTINGS_APPEARANCE_PAGE_TITLE;
		}
	}
}
