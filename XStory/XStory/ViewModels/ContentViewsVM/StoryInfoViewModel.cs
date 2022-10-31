using Prism.Commands;
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

        private bool _isChapterListVisible;
        public bool IsChapterListVisible
        {
            get { return _isChapterListVisible; }
            set { SetProperty(ref _isChapterListVisible, value); }
        }

        public DelegateCommand<string> ChapterSelectionCommand { get; set; }

        public StoryInfoViewModel(INavigationService navigationService) : base(navigationService)
        {
            IsChapterListVisible = true;

            ChapterSelectionCommand = new DelegateCommand<string>((url)=>ExecuteChapterSelectionCommand(url));
        }

        private async void ExecuteChapterSelectionCommand(string url)
        {
            var navigationParams = new NavigationParameters()
            {
                { "storyUrl", url }
            };

            await NavigationService.NavigateAsync("StoryPage", navigationParams);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Story = parameters.GetValue<Story>("story");
                if (Story != null)
                {
                    Title = Story.Title;

                    if (Story.ChaptersList.Count == 0)
                    {
                        IsChapterListVisible = false;
                    } 
                }
            }
            catch (Exception e)
            {
                Story = null;
            }

        }
    }
}
