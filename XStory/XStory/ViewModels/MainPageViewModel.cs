using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using XStory.DTO;
using XStory.Helpers;

namespace XStory.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region --- Fields ---
        private List<Story> _stories;
        private BL.Web.Contracts.IServiceStory _serviceStory;

        private int _pageNumber;

        public DelegateCommand LoadMoreStoriesCommand { get; set; }
        public DelegateCommand<string> StoriesItemTappedCommand { get; set; }
        public DelegateCommand StoriesItemAppearingCommand { get; set; }

        public DelegateCommand StoriesRefreshCommand { get; set; }

        public DelegateCommand SettingsCommand { get; set; }

        public List<Story> Stories
        {
            get { return _stories; }
            set { SetProperty(ref _stories, value); }
        }

        private bool _isStoriesListRefreshing;
        public bool IsStoriesListRefreshing
        {
            get { return _isStoriesListRefreshing; }
            set { SetProperty(ref _isStoriesListRefreshing, value); }
        }

        #endregion

        public MainPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory)
            : base(navigationService)
        {
            Title = "Main Page";

            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            LoadMoreStoriesCommand = new DelegateCommand(ExecuteLoadMoreStoriesCommand);
            SettingsCommand = new DelegateCommand(ExecuteSettingsCommand);
            StoriesItemTappedCommand = new DelegateCommand<string>((url) => ExecuteStoriesItemTappedCommand(url));
            StoriesItemAppearingCommand = new DelegateCommand(ExecuteStoriesItemAppearingCommand);
            StoriesRefreshCommand = new DelegateCommand(ExecuteStoriesRefreshCommand);

            _pageNumber = 0;

            _serviceStory = serviceStory;
        }

        private void ExecuteStoriesItemAppearingCommand()
        {
            string s = "coucou";
        }

        private async void ExecuteSettingsCommand()
        {
            await NavigationService.NavigateAsync("SettingsPage");
        }

        private async void ExecuteStoriesItemTappedCommand(string url)
        {
            var navigationParams = new NavigationParameters()
            {
                { "storyUrl", url }
            };

            await NavigationService.NavigateAsync("StoryPage", navigationParams);
        }

        /// <summary>
        /// When the PullToRefresh command is triggered on StoriesList.
        /// If the 1st item of the two lists is different : refresh. 
        /// Else : nothing
        /// </summary>
        private async void ExecuteStoriesRefreshCommand()
        {
            try
            {
                IsStoriesListRefreshing = true;

                List<Story> storiesRefresh = await _serviceStory.GetStoriesMainPage(0, "");
                if (storiesRefresh != null && storiesRefresh.Count > 0)
                {
                    if (Stories != null && Stories.Count > 0)
                    {
                        if (Stories.First().Url != storiesRefresh.First().Url)
                        {
                            // if 1st's are differents : refresh
                            Stories = storiesRefresh;
                        }
                        // else : nothing because no need to refresh
                    }
                    // else : nothing because the main Stories list is bad.
                }
                // else : nothing because the refresh failed.

                IsStoriesListRefreshing = false;
            }
            catch (Exception ex)
            {
                IsStoriesListRefreshing = false;
            }
            AppSettings.FirstRun = false;
        }

        /// <summary>
        /// First appearing.
        /// If no Stories : get stories
        /// Else : do nothing
        /// </summary>
        protected override async void ExecuteAppearingCommand()
        {
            // Have to call InitTheming() everytime VM appears because of this stupid Android BackButton issue
            InitTheming();

            if (Stories == null || Stories.Count == 0)
            {
                Stories = await _serviceStory.GetStoriesMainPage(_pageNumber, "");
            }
        }

        private async void ExecuteLoadMoreStoriesCommand()
        {
            //_pageNumber++;

            //if (Stories != null && Stories.Count > 0)
            //{
            //    List<Story> storiesNext = await _serviceStory.GetStoriesMainPage(_pageNumber, "");
            //    Stories.AddRange(storiesNext);
            //}
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
