using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using XStory.DTO;
using XStory.Helpers;
using XStory.Helpers.Constants;

namespace XStory.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region --- Fields ---
        private IPageDialogService _pageDialogService;

        private ObservableCollection<Story> _stories;
        private BL.Web.Contracts.IServiceStory _serviceStory;
        private BL.Web.Contracts.IServiceCategory _serviceCategoryWeb;

        private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

        private int _pageNumber;
        private string[] _hiddenCategories;

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

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, BL.Web.Contracts.IServiceStory serviceStory, BL.Web.Contracts.IServiceCategory serviceCategoryWeb, BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;

            Title = MainPageConstants.MAINPAGE_TITLE;
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
            _hiddenCategories = new string[] { };

            if (AppSettings.FirstRun)
            {
                // If FIRST run : diclaimer Message "Welcome" + "disabled categories"
                this.DisplayFirstRunMessage();

                AppSettings.FirstRun = false;
            }

            InitStories();
            InitCategories();
        }

        private void DisplayFirstRunMessage()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await _pageDialogService.DisplayAlertAsync(
                    MainPageConstants.MAINPAGE_FIRST_RUN_TITLE,
                    MainPageConstants.MAINPAGE_FIRST_RUN_MESSAGE,
                    "OK");
            });
        }

        private void ExecuteStoriesItemAppearingCommand()
        {
            // string s = "coucou";
        }

        private async void ExecuteSettingsCommand()
        {
            await NavigationService.NavigateAsync(nameof(Views.SettingsPage));
        }

        private async void ExecuteStoriesItemTappedCommand(string url)
        {
            var navigationParams = new NavigationParameters()
            {
                { "storyUrl", url }
            };

            await NavigationService.NavigateAsync(nameof(Views.StoryPage), navigationParams);
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
                        {// if 1st's are differents : refresh

                            // filter
                            storiesRefresh = _serviceStory.FilterStories(storiesRefresh, _hiddenCategories);
                            Stories = new ObservableCollection<Story>(storiesRefresh);

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
                Logger.ServiceLog.Error(ex);
                AppSettings.HiddenCategoriesChanged = false;
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
            if (AppSettings.HiddenCategoriesChanged)
            {
                InitStories();
            }
        }

        private async void ExecuteLoadMoreStoriesCommand()
        {
            _pageNumber++;

            if (Stories != null && Stories.Count > 0)
            {
                List<Story> storiesNext = await _serviceStory.GetFilteredStoriesMainPage(_pageNumber, _hiddenCategories, "");


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
            if (AppSettings.HiddenCategoriesChanged || (Stories == null || Stories.Count == 0))
            {
                ViewState = ViewStateEnum.Loading;
                //CheckCategories
                try
                {
                    var categories = await _serviceCategorySQLite.GetCategories();
                    _hiddenCategories = categories.Where(c => !c.IsEnabled).Select(c => c.Url).ToArray();

                    if (_hiddenCategories != null || _hiddenCategories.Length > 0)
                    {
                        var filteredStories = await _serviceStory.GetFilteredStoriesMainPage(_pageNumber, _hiddenCategories, "");
                        Stories = new ObservableCollection<Story>(filteredStories);
                    }
                    else
                    {
                        var stories = await _serviceStory.GetStoriesMainPage(_pageNumber, "");
                        Stories = new ObservableCollection<Story>(stories);
                    }
                    ViewState = ViewStateEnum.Display;
                }
                catch (Exception ex)
                {
                    Logger.ServiceLog.Error(ex);
                    ViewState = ViewStateEnum.Error;
                }
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
                Logger.ServiceLog.Error(ex);
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
