using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XStory.DTO;
using XXRead.Helpers;
using XXRead.Helpers.Constants;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        #region --- Fields ---
        private XStory.BL.Common.Contracts.IServiceStory _serviceStory;
        private XStory.BL.Common.Contracts.IServiceCategory _serviceCategory;
        private XStory.BL.Common.Contracts.IServiceConfig _serviceConfig;

        private IServicePopup _servicePopup;

        private Story _selectedStory;
        public Story SelectedStory
        {
            get { return _selectedStory; }
            set { SetProperty(ref _selectedStory, value); }
        }

        private ObservableCollection<Story> _stories;

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
            set { SetProperty(ref _currentCategory, value); }
        }

        #endregion

        #region --- Commands ---
        public RelayCommand CategoryTappedCommand { get; set; }
        public RelayCommand GetDbStoriesCommand { get; set; }
        public RelayCommand LoadMoreStoriesCommand { get; set; }
        public RelayCommand<Story> StoriesItemTappedCommand { get; set; }
        public RelayCommand<Story> LongPressStoryCommand { get; set; }
        public RelayCommand StoriesRefreshCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        #endregion

        #region --- Ctor ---
        public MainPageViewModel(IServiceNavigation serviceNavigation,
            IServicePopup servicePopup,
            XStory.BL.Common.Contracts.IServiceStory serviceStory,
            XStory.BL.Common.Contracts.IServiceCategory serviceCategory,
            XStory.BL.Common.Contracts.IServiceConfig serviceConfig) : base(serviceNavigation)
        {
            _serviceStory = serviceStory;
            _serviceCategory = serviceCategory;
            _serviceConfig = serviceConfig;

            _servicePopup = servicePopup;

            Title = MainPageConstants.MAINPAGE_TITLE;
            ViewState = ViewStateEnum.Loading;

            AppearingCommand = new RelayCommand(ExecuteAppearingCommand);
            CategoryTappedCommand = new RelayCommand(ExecuteCategoryTappedCommand);
            GetDbStoriesCommand = new RelayCommand(ExecuteGetDbStoriesCommand);
            LoadMoreStoriesCommand = new RelayCommand(ExecuteLoadMoreStoriesCommand);
            SettingsCommand = new RelayCommand(ExecuteSettingsCommand);
            StoriesItemTappedCommand = new RelayCommand<Story>((story) => ExecuteStoriesItemTappedCommand(story));
            LongPressStoryCommand = new RelayCommand<Story>((story) => ExecuteLongPressStoryCommand(story));
            StoriesRefreshCommand = new RelayCommand(ExecuteStoriesRefreshCommand);
            TryAgainCommand = new RelayCommand(ExecuteTryAgainCommand);

            if (AppSettings.FirstRun)
            {
                // If FIRST run : diclaimer Message "Welcome"
                this.DisplayFirstRunMessage();

                AppSettings.FirstRun = false;
            }

            this.InitData();
        }
        #endregion

        private void InitData()
        {
            _serviceConfig.SetCurrentDataSource((XStory.DTO.Config.DataSources)Enum.Parse(typeof(XStory.DTO.Config.DataSources), AppSettings.DataSource));

            Task.Run(this.InitCategories).Wait();
            Task.Run(() => this.InitStories()).Wait();
        }

        [Obsolete("Deprecated. (to improvise - with Task.ContinueWith()) - Use InitData instead")]
        private void InitDataOld()
        {
            this.InitCategories().ContinueWith(result =>
                {
                    if (result.Status == TaskStatus.RanToCompletion)
                    {
                        this.InitStories();
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

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.Windows[0].Page.DisplayAlertAsync(
                    MainPageConstants.MAINPAGE_FIRST_RUN_TITLE,
                    MainPageConstants.MAINPAGE_FIRST_RUN_MESSAGE,
                    "OK");
                //await Shell.Current.DisplayAlert(
                //    MainPageConstants.MAINPAGE_FIRST_RUN_TITLE,
                //    MainPageConstants.MAINPAGE_FIRST_RUN_MESSAGE,
                //    "OK");
            });
        }

        /// <summary>
        /// <br>First appearing.</br>
        /// <br>Init Color themes.</br>
        /// </summary>
        protected override void ExecuteAppearingCommand()
        {
            // Have to call InitTheming() everytime VM appears because of this stupid Android BackButton issue
            //this.InitTheming();

            if (AppSettings.DataSourceChanged)
            {
                Stories = null;
                this.InitData();
            }
        }

        /// <summary>
        /// Display Categories selection popup
        /// </summary>
        private async void ExecuteCategoryTappedCommand()
        {
            _currentCategory = _serviceCategory.GetCurrentCategory();

            var result = await _servicePopup.ShowPopupAsync<Views.Popup.PopupSelectCategoryPage, ViewModels.PopupViewModels.PopupSelectCategoryPageViewModel, XStory.DTO.Category>();
            if (result.WasDismissedByTappingOutsideOfPopup)
            {
                return;
            }
            // null = "all categories"
            XStory.DTO.Category selectedCategory = result.Result;

            if (selectedCategory != _currentCategory)
            {
                CurrentCategory = selectedCategory;
                _serviceCategory.SetCurrentCategory(selectedCategory);
                Stories = new ObservableCollection<Story>(await _serviceStory.InitStories());
            }
        }

        private async void ExecuteSettingsCommand()
        {
            //await _servicePopup.ShowPopupAsync<ViewModels.PopupViewModels.PopupFlyoutMenuPageViewModel>();
            await ServiceNavigation.NavigateTo<Views.SettingsPage>();
        }

        private async void ExecuteStoriesItemTappedCommand(Story story)
        {
            if (story != null)
            {
                _serviceStory.SetCurrentStory(story);
            }

            await ServiceNavigation.NavigateTo<Views.StoryPage>();
        }

        private async void ExecuteLongPressStoryCommand(Story story)
        {

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
                if (Stories == null || Stories.Count == 0)
                {
                    Stories = new ObservableCollection<Story>(await _serviceStory.InitStories());
                    return;
                    // OU Exception ?
                }
                IsStoriesListRefreshing = true;

                List<Story> refreshList = await _serviceStory.RefreshStories(Stories.First());
                if (refreshList == null)
                {
                    throw new Exception("Couldn't refresh stories");
                }
                else if (refreshList.Count > 0)
                {
                    Stories = new ObservableCollection<Story>(refreshList);
                }

                IsStoriesListRefreshing = false;
            }
            catch (Exception ex)
            {
                XStory.Logger.ServiceLog.Error(ex);
                IsStoriesListRefreshing = false;
            }
        }

        private async void ExecuteLoadMoreStoriesCommand()
        {
            var moreStories = await _serviceStory.LoadMoreStories();

            moreStories.ForEach(story => _stories.Add(story));

            Stories = new ObservableCollection<Story>(_serviceStory.DistinctStories(_stories.ToList()));
        }

        /// <summary>
        /// Get stories on Main Page.
        /// </summary>
        /// <param name="forceInit">Force (re)init if true.</param>
        private async void InitStories(bool forceInit = false)
        {
            if (forceInit || (Stories == null || Stories.Count == 0))
            {
                ViewState = ViewStateEnum.Loading;
                try
                {
                    Stories = new ObservableCollection<Story>(await _serviceStory.InitStories());
                    ViewState = ViewStateEnum.Display;
                }
                catch (Exception ex)
                {
                    XStory.Logger.ServiceLog.Error(ex);
                    ViewState = ViewStateEnum.Error;
                }
                AppSettings.DataSourceChanged = false;
            }
        }

        private async void ExecuteGetDbStoriesCommand()
        {
            if (Title == MainPageConstants.MAINPAGE_TITLE)
            {
                var storiess = await _serviceStory.GetStoriesSQLite();
                Stories = new ObservableCollection<Story>(storiess);
                Title = MainPageConstants.MAINPAGE_TITLE_SAVED;
            }
            else
            {
                this.InitStories(true);
                Title = MainPageConstants.MAINPAGE_TITLE;
            }
        }

        /// <summary>
        /// Get Categories from web and insert it in the database.
        /// </summary>
        private async Task InitCategories()
        {
            try
            {
                await _serviceCategory.InitCategories();
            }
            catch (Exception ex)
            {
                XStory.Logger.ServiceLog.Error(ex);
            }
        }

        protected override void ExecuteTryAgainCommand()
        {
            this.InitStories();
        }

        private void OnCurrentCategoryChanged()
        {
            _serviceStory.ResetPageNumber();

            this.InitStories(true);
        }
    }
}
