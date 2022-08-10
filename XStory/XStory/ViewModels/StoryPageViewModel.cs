using Prism.Commands;
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

        private Story _story;
        public Story Story
        {
            get { return _story; }
            set { SetProperty(ref _story, value); }
        }

        string storyUrl = string.Empty;

        public StoryPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory)
            : base(navigationService)
        {
            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);

            _serviceStory = serviceStory;
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
    }
}
