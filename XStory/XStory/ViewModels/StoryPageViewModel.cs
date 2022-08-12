﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.ViewModels
{
    public class StoryPageViewModel : BaseViewModel
    {
        private BL.Web.Contracts.IServiceStory _serviceStory;

        private bool _isStoryInfoVisible;
        public bool IsStoryInfoVisible
        {
            get { return _isStoryInfoVisible; }
            set { SetProperty(ref _isStoryInfoVisible, value); }
        }

        private Story _story;
        public Story Story
        {
            get { return _story; }
            set { SetProperty(ref _story, value); }
        }
        public DelegateCommand DisplayStoryInfoCommand { get; set; }

        string storyUrl = string.Empty;

        public StoryPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory)
            : base(navigationService)
        {
            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            DisplayStoryInfoCommand = new DelegateCommand(ExecuteDisplayStoryInfoCommand);

            _serviceStory = serviceStory;
        }

        private async void ExecuteDisplayStoryInfoCommand()
        {
            INavigationParameters navigationParameters = new NavigationParameters()
            {
                { "story" , Story }
            };

            await NavigationService.NavigateAsync("StoryInfoView", navigationParameters, useModalNavigation: true);
            //if (IsStoryInfoVisible)
            //{
            //    IsStoryInfoVisible = false;
            //}
            //else
            //{
            //    IsStoryInfoVisible = true;

            //}
        }

        protected override async void ExecuteAppearingCommand()
        {
            if (!string.IsNullOrWhiteSpace(storyUrl))
            {
                Story = await _serviceStory.GetStory(storyUrl);
            }

            if (Story != null)
            {
                Title = Story.Title;
            }
        }


        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                storyUrl = parameters.GetValue<string>("storyUrl");
            }
            catch (Exception e)
            {
                storyUrl = null;
            }

        }
    }
}
