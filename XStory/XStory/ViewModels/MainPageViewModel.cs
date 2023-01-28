using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<Story> _stories;
        private BL.Web.Contracts.IServiceStory _serviceStory;
        private BL.Web.Contracts.IServiceCategory _serviceCategoryWeb;

        private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

        private int _pageNumber;

        public DelegateCommand LoadMoreStoriesCommand { get; set; }
        public DelegateCommand<string> StoriesItemTappedCommand { get; set; }
        public DelegateCommand StoriesItemAppearingCommand { get; set; }

        public DelegateCommand StoriesRefreshCommand { get; set; }

        public DelegateCommand SettingsCommand { get; set; }

        public ObservableCollection<Story> Stories
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

        public MainPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory, BL.Web.Contracts.IServiceCategory serviceCategoryWeb, BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite)
            : base(navigationService)
        {
            Title = "Main Page";
            ViewState = ViewStateEnum.Loading;

            _serviceStory = serviceStory;
            _serviceCategoryWeb = serviceCategoryWeb;
            _serviceCategorySQLite = serviceCategorySQLite;

            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            LoadMoreStoriesCommand = new DelegateCommand(ExecuteLoadMoreStoriesCommand);
            SettingsCommand = new DelegateCommand(ExecuteSettingsCommand);
            StoriesItemTappedCommand = new DelegateCommand<string>((url) => ExecuteStoriesItemTappedCommand(url));
            StoriesItemAppearingCommand = new DelegateCommand(ExecuteStoriesItemAppearingCommand);
            StoriesRefreshCommand = new DelegateCommand(ExecuteStoriesRefreshCommand);
            TryAgainCommand = new DelegateCommand(InitStories);

            _pageNumber = 1;

            InitStories();
            InitCategories();

            // If FIRST run : diclaimer Message "Welcome" + "disabled categories"
        }

        private void ExecuteStoriesItemAppearingCommand()
        {
            // string s = "coucou";
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

                List<Story> storiesRefresh = await _serviceStory.GetStoriesMainPage(1, "");
                if (storiesRefresh != null && storiesRefresh.Count > 0)
                {
                    if (Stories != null && Stories.Count > 0)
                    {
                        if (Stories.First().Url != storiesRefresh.First().Url)
                        {
                            // if 1st's are differents : refresh
                            Stories.Clear();
                            foreach (var story in storiesRefresh)
                            {
                                Stories.Add(story);
                            }
                            _pageNumber = 1;
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
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                IsStoriesListRefreshing = false;
            }
        }

        /// <summary>
        /// <br>First appearing.</br>
        /// <br>Init Color themes.</br>
        /// </summary>
        protected override void ExecuteAppearingCommand()
        {
            // Have to call InitTheming() everytime VM appears because of this stupid Android BackButton issue
            InitTheming();
        }

        private async void ExecuteLoadMoreStoriesCommand()
        {
            _pageNumber++;

            if (Stories != null && Stories.Count > 0)
            {
                List<Story> storiesNext = await _serviceStory.GetStoriesMainPage(_pageNumber, "");
                if (storiesNext != null)
                {
                    foreach (var storyNext in storiesNext)
                    {
                        Stories.Add(storyNext);
                    }
                }
            }
        }

        /// <summary>
        /// Get stories on Main Page.
        /// </summary>
        private async void InitStories()
        {
            try
            {
                if (Stories == null || Stories.Count == 0)
                {
                    ViewState = ViewStateEnum.Loading;

                    Stories = new ObservableCollection<Story>(await _serviceStory.GetStoriesMainPage(_pageNumber, ""));
                    ViewState = ViewStateEnum.Display;
                }
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                ViewState = ViewStateEnum.Error;
            }
        }

        /// <summary>
        /// Get Categories from web and insert it in the database.
        /// </summary>
        private async void InitCategories()
        {
            try
            {
                bool hasDbCategories = await _serviceCategorySQLite.HasDBCategories();

                if (!hasDbCategories)
                {
                    List<DTO.Category> categories = await _serviceCategoryWeb.GetCategories();
                    await _serviceCategorySQLite.InsertCategories(categories);
                }
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
            }
        }

        //protected override void ExecuteTryAgainCommand()
        //{
        //    InitStories();
        //}

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
