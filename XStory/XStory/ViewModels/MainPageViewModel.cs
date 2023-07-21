using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        //private BL.Web.XStory.Contracts.IServiceStory _serviceStory;
        private BL.Web.XStory.Contracts.IServiceCategory _serviceCategoryWeb;

        private BL.Web.DSLocator.Contracts.IServiceStory _dsServiceStory;

        private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

        private int _pageNumber;
        private List<string> _hiddenCategories;

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

        private Category _currentCategory;
        public Category CurrentCategory
        {
            get { return _currentCategory; }
            set
            {
                SetProperty(ref _currentCategory, value);
                OnCurrentCategoryChanged();
            }
        }

        #endregion

        #region --- Commands ---
        public DelegateCommand CategoryTappedCommand { get; set; }
        public DelegateCommand LoadMoreStoriesCommand { get; set; }
        public DelegateCommand<string> StoriesItemTappedCommand { get; set; }
        public DelegateCommand StoriesRefreshCommand { get; set; }
        public DelegateCommand SettingsCommand { get; set; }
        #endregion

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, BL.Web.DSLocator.Contracts.IServiceStory dsServiceStory/*, BL.Web.XStory.Contracts.IServiceStory serviceStory, */, BL.Web.XStory.Contracts.IServiceCategory serviceCategoryWeb, BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;

            Title = MainPageConstants.MAINPAGE_TITLE;
            ViewState = ViewStateEnum.Loading;

            _dsServiceStory = dsServiceStory;
            _serviceCategoryWeb = serviceCategoryWeb;
            _serviceCategorySQLite = serviceCategorySQLite;

            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            CategoryTappedCommand = new DelegateCommand(ExecuteCategoryTappedCommand);
            LoadMoreStoriesCommand = new DelegateCommand(ExecuteLoadMoreStoriesCommand);
            SettingsCommand = new DelegateCommand(ExecuteSettingsCommand);
            StoriesItemTappedCommand = new DelegateCommand<string>((url) => ExecuteStoriesItemTappedCommand(url));
            StoriesRefreshCommand = new DelegateCommand(ExecuteStoriesRefreshCommand);
            TryAgainCommand = new DelegateCommand(ExecuteTryAgainCommand);

            _pageNumber = 1;
            _hiddenCategories = new List<string>();

            if (AppSettings.FirstRun)
            {
                // If FIRST run : diclaimer Message "Welcome" + "disabled categories"
                this.DisplayFirstRunMessage();

                AppSettings.FirstRun = false;
            }

            InitCategories()
                .ContinueWith(result =>
                {
                    if (result.Status == TaskStatus.RanToCompletion)
                    {
                        InitHiddenCategories().ContinueWith(res =>
                        {
                            if (res.Status == TaskStatus.RanToCompletion)
                            {
                                InitStories();
                            }
                        });
                    }
                });
        }

        private void DisplayFirstRunMessage()
        {
            // SUPER UGLY !
            // SUPER UGLY !
            // What about a beautiful welcome page :)
            // SUPER UGLY !
            // SUPER UGLY !
            Device.BeginInvokeOnMainThread(async () =>
            {
                await _pageDialogService.DisplayAlertAsync(
                    MainPageConstants.MAINPAGE_FIRST_RUN_TITLE,
                    MainPageConstants.MAINPAGE_FIRST_RUN_MESSAGE,
                    "OK");
            });
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

        private async void ExecuteCategoryTappedCommand()
        {
            // Display Categories selection popup
            await NavigationService.NavigateAsync(nameof(Views.Popup.PopupSelectCategoryPage));
        }

        private async void ExecuteSettingsCommand()
        {
            await NavigationService.NavigateAsync(nameof(Views.SettingsPage2));
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

                List<Story> storiesRefresh = await _dsServiceStory.GetStoriesPage(this.DataSource, 1);
                if (storiesRefresh != null && storiesRefresh.Count > 0)
                {
                    if (Stories != null && Stories.Count > 0)
                    {
                        if (Stories.First().Url != storiesRefresh.First().Url)
                        {// if 1st's are differents : refresh

                            // filter
                            storiesRefresh = _dsServiceStory.FilterStories(this.DataSource, storiesRefresh, _hiddenCategories);
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
                IsStoriesListRefreshing = false;
            }
        }

        private async void ExecuteLoadMoreStoriesCommand()
        {
            _pageNumber++;

            if (Stories != null && Stories.Count > 0)
            {
                List<Story> storiesNext = await _dsServiceStory.GetStoriesPage(this.DataSource, _pageNumber, CurrentCategory?.Url);

                if (CurrentCategory == null)
                {
                    // If CurrentCategory is defined, no need to filter because the category is already chosen, so valid
                    storiesNext = _dsServiceStory.FilterStories(this.DataSource, storiesNext, _hiddenCategories);
                }

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
        private async void InitStories(bool forceInit = false)
        {
            if (AppSettings.HiddenCategoriesChanged)
            {
                forceInit = true;
                _pageNumber = 1;
            }

            if (forceInit || (Stories == null || Stories.Count == 0))
            {
                ViewState = ViewStateEnum.Loading;
                try
                {
                    var stories = await _dsServiceStory.GetStoriesPage(this.DataSource, _pageNumber, CurrentCategory?.Url);

                    if (CurrentCategory == null)
                    {
                        // If CurrentCategory is defined, no need to filter because the category is already chosen, so valid
                        _hiddenCategories = await _serviceCategorySQLite.GetHiddenCategories();
                        stories = _dsServiceStory.FilterStories(this.DataSource, stories, _hiddenCategories);
                    }

                    Stories = new ObservableCollection<Story>(stories);

                    ViewState = ViewStateEnum.Display;
                    AppSettings.HiddenCategoriesChanged = false;
                }
                catch (Exception ex)
                {
                    Logger.ServiceLog.Error(ex);
                    ViewState = ViewStateEnum.Error;
                    AppSettings.HiddenCategoriesChanged = false;
                }
            }
        }

        /// <summary>
        /// Get Categories from web and insert it in the database.
        /// </summary>
        private async Task InitCategories()
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

        private async Task InitHiddenCategories()
        {
            _hiddenCategories = await _serviceCategorySQLite.GetHiddenCategories();
        }

        protected override void ExecuteTryAgainCommand()
        {
            InitStories(false);
        }

        private void OnCurrentCategoryChanged()
        {
            _pageNumber = 1;
            InitStories(true);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("selectedCategory"))
            {
                try
                {
                    if (parameters.ContainsKey("resetCategories"))
                    {
                        CurrentCategory = null;
                        return;
                    }

                    var selectedCategory = parameters.GetValue<Category>("selectedCategory");
                    if (selectedCategory != null)
                    {
                        CurrentCategory = selectedCategory;

                    }
                }
                catch (Exception ex)
                {
                    Logger.ServiceLog.Error(ex);
                }
            }
        }
    }
}
