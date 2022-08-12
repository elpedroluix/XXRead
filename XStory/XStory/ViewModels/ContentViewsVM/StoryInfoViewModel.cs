using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using XStory.DTO;

namespace XStory.ViewModels.ContentViewsVM
{
    public class StoryInfoViewModel : BaseViewModel
    {
        private Story _story;
        public Story Story
        {
            get { return _story; }
            set { SetProperty(ref _story, value); }
        }

        public StoryInfoViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Story = parameters.GetValue<Story>("story");
            }
            catch (Exception e)
            {
                Story = null;
            }

        }
    }
}
