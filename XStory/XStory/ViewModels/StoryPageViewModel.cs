using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        public DelegateCommand ShareStoryCommand { get; set; }

        string storyUrl = string.Empty;

        public StoryPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory, BL.SQLite.Contracts.IServiceSettings serviceSettings)
            : base(navigationService)
        {
            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            DisplayStoryInfoCommand = new DelegateCommand(ExecuteDisplayStoryInfoCommand);
            ShareStoryCommand = new DelegateCommand(ExecuteShareStoryCommand);


            _serviceStory = serviceStory;
        }

        private async void ExecuteDisplayStoryInfoCommand()
        {
            INavigationParameters navigationParameters = new NavigationParameters()
            {
                { "story" , Story }
            };

            await NavigationService.NavigateAsync("StoryInfoPage", navigationParameters);
        }

        protected override async void ExecuteAppearingCommand()
        {
            if (!string.IsNullOrWhiteSpace(storyUrl))
            {
                Story = await _serviceStory.GetStory(storyUrl);

                if (Story != null)
                {
                    Title = Story.Title;
                }
            }
        }

        private async void ExecuteShareStoryCommand()
        {
            try
            {
                await Share.RequestAsync(new ShareTextRequest()
                {
                    Uri = Story.Url,
                    Text = Story.Title,
                    Title = "Partager ce récit"
                });
            }
            catch (Exception ex)
            {
                XStory.Logger.ServiceLog.Log("Error", "Couldn't share story url", this.GetType().Name, DateTime.Now, Logger.LogType.Error);
                return;
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
