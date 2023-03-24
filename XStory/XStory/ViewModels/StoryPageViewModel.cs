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
using XStory.Helpers;

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
        public DelegateCommand<string> ChapterArrowTapped { get; set; }
        public DelegateCommand DisplayStoryInfoCommand { get; set; }
        public DelegateCommand ShareStoryCommand { get; set; }

        string storyUrl = string.Empty;

        public StoryPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory, BL.SQLite.Contracts.IServiceSettings serviceSettings)
            : base(navigationService)
        {
            _serviceStory = serviceStory;
            ViewState = Helpers.ViewStateEnum.Loading;

            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            ChapterArrowTapped = new DelegateCommand<string>((direction) => ExecuteChapterArrowTappedCommand(direction));
            DisplayStoryInfoCommand = new DelegateCommand(ExecuteDisplayStoryInfoCommand);
            ShareStoryCommand = new DelegateCommand(ExecuteShareStoryCommand);
            TryAgainCommand = new DelegateCommand(InitStory);
        }

        private void ExecuteChapterArrowTappedCommand(string direction)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(direction)) throw new ArgumentNullException();

                Story chapter = null;

                switch (direction)
                {
                    case "left":
                        int previousChapter = Story.ChapterNumber - 1;
                        chapter = Story.ChaptersList.ElementAt(previousChapter - 1);
                        break;

                    case "right":
                        int nextChapter = Story.ChapterNumber + 1;
                        chapter = Story.ChaptersList.ElementAt(nextChapter - 1);
                        break;
                }

                if (chapter != null)
                {
                    // TODO : optimiser !!!
                    storyUrl = chapter.Url;
                    Story = chapter;

                    ViewState = Helpers.ViewStateEnum.Loading;

                    InitStory();
                }
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Error(ex);
            }
        }

        private async void ExecuteDisplayStoryInfoCommand()
        {
            INavigationParameters navigationParameters = new NavigationParameters()
            {
                { "story" , Story }
            };

            await NavigationService.NavigateAsync(nameof(Views.StoryInfoPage), navigationParameters);
        }

        /// <summary>
        /// Gets the Story from either the cache if exists, or from the web if it does not.
        /// </summary>
        private async void InitStory()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(storyUrl))
                {
                    var alreadyLoadedStory = StaticContext.ListAlreadyLoadedStories.FirstOrDefault(story => story.Url.Contains(storyUrl));
                    if (alreadyLoadedStory != null)
                    {
                        Story = alreadyLoadedStory;
                    }
                    else
                    {
                        Story = await _serviceStory.GetStory(storyUrl);
                    }

                    if (Story != null)
                    {
                        Title = Story.Title;
                        if (alreadyLoadedStory == null && StaticContext.ListAlreadyLoadedStories.FirstOrDefault(story => story.Url.Contains(storyUrl)) == null)
                        {
                            // If Story does not exists in StaticContext.ListAlreadyLoadedStories -> add in cache
                            StaticContext.ListAlreadyLoadedStories.Add(Story);
                        }
                        ViewState = Helpers.ViewStateEnum.Display;
                    }
                    else
                    {
                        ViewState = Helpers.ViewStateEnum.Error;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Error(ex);
                ViewState = Helpers.ViewStateEnum.Error;
            }

        }

        protected override void ExecuteAppearingCommand()
        {
            // InitStory is called here because of property storyUrl initialized during OnNavigatedTo
            this.InitStory();

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
                Logger.ServiceLog.Error(ex);
                return;
            }

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(storyUrl))
                {
                    storyUrl = parameters.GetValue<string>("storyUrl");
                }
            }
            catch (Exception e)
            {
                storyUrl = null;
            }

        }
    }
}
