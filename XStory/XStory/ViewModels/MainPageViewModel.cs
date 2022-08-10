﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XStory.DTO;

namespace XStory.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private List<Story> _stories;
        private BL.Web.Contracts.IServiceStory _serviceStory;

        public DelegateCommand<string> StoriesItemTappedCommand { get; set; }

        public List<Story> Stories
        {
            get { return _stories; }
            set { SetProperty(ref _stories, value); }
        }

        public MainPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory)
            : base(navigationService)
        {
            Title = "Main Page";

            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            StoriesItemTappedCommand = new DelegateCommand<string>((url) => ExecuteStoriesItemTappedCommand(url));

            _serviceStory = serviceStory;
        }

        private async void ExecuteStoriesItemTappedCommand(string url)
        {
            var navigationParams = new NavigationParameters()
            {
                { "storyUrl", url }
            };

            await NavigationService.NavigateAsync("StoryPage", navigationParams);
        }

        protected override async void ExecuteAppearingCommand()
        {
            Stories = await _serviceStory.GetStoriesMainPage(0, "");
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
