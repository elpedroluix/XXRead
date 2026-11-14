using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.Helpers;
using XStory.Logger;
using CommunityToolkit.Mvvm.Input;
using XXRead.Helpers.Services;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace XXRead.ViewModels
{
    public class StoryPageViewModel : Common.BaseStoryViewModel
    {
        #region --- Fields ---
        private IServicePopup _servicePopup;

        private IServiceStory _serviceStory;
        private IServiceAuthor _serviceAuthor;

        private bool _isStoryInfoVisible;
        public bool IsStoryInfoVisible
        {
            get { return _isStoryInfoVisible; }
            set { SetProperty(ref _isStoryInfoVisible, value); }
        }

        /// <summary>
        /// Story from current StoryPageViewModel. To keep Story from this VM when/if other StoryPageViewModel is instanciated
        /// </summary>
        private Story _storyKeep;
        #endregion

        #region --- Commands ---
        public RelayCommand AuthorTappedCommand { get; set; }
        public RelayCommand<string> ChapterArrowTapped { get; set; }
        public RelayCommand ChapterNameTappedCommand { get; set; }
        public RelayCommand DisplayStoryInfoCommand { get; set; }
        public RelayCommand OpenStoryActionsCommand { get; set; }
        public RelayCommand OpenInBrowserCommand { get; set; }
        public RelayCommand ToggleStoryInfosCommand { get; set; }
        #endregion

        #region --- Ctor ---
        public StoryPageViewModel(IServiceNavigation serviceNavigation,
            IServicePopup servicePopup,
            IServiceStory serviceStory,
            IServiceAuthor serviceAuthor)
            : base(serviceNavigation, serviceStory)
        {
            _servicePopup = servicePopup;

            _serviceStory = serviceStory;
            _serviceAuthor = serviceAuthor;

            AppearingCommand = new RelayCommand(ExecuteAppearingCommand);
            AuthorTappedCommand = new RelayCommand(ExecuteAuthorTappedCommand);
            ChapterArrowTapped = new RelayCommand<string>((direction) => ExecuteChapterArrowTappedCommand(direction));
            ChapterNameTappedCommand = new RelayCommand(ExecuteChapterNameTappedCommand);
            DisplayStoryInfoCommand = new RelayCommand(ExecuteDisplayStoryInfoCommand);
            OpenStoryActionsCommand = new RelayCommand(ExecuteOpenStoryActionsCommand);
            OpenInBrowserCommand = new RelayCommand(ExecuteOpenInBrowserCommand);
            ToggleStoryInfosCommand = new RelayCommand(ExecuteToggleStoryInfosCommand);
            TryAgainCommand = new RelayCommand(InitStory);

            this.InitStory();
        }
        #endregion

        /// <summary>
        /// Navigates to Author's page. If already from same Author page, go back.
        /// </summary>
        private async void ExecuteAuthorTappedCommand()
        {
            if (Story.Author != null)
            {
                _serviceAuthor.SetCurrentAuthor(Story.Author);

                bool hasAuthorPage;
                try
                {
                    hasAuthorPage = Application.Current.Windows[0].Page.Navigation.NavigationStack.Any(p => p.GetType() == typeof(Views.AuthorPage));
                }
                catch (Exception ex)
                {
                    hasAuthorPage = false;
                    ServiceLog.Error(ex);
                }
                if (hasAuthorPage)
                {
                    // if AuthorPage exists : same Author so, go back to AuthorPage
                    await ServiceNavigation.GoBack();
                    return;
                }

                _storyKeep = Story;
                await ServiceNavigation.NavigateTo<Views.AuthorPage>();
            }
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
                    _serviceStory.SetCurrentStory(chapter);

                    ViewState = Helpers.ViewStateEnum.Loading;

                    this.InitStory();
                }
            }
            catch (Exception ex)
            {
                XStory.Logger.ServiceLog.Error(ex);
            }
        }

        private async void ExecuteChapterNameTappedCommand()
        {
            if (Story != null && Story.ChaptersList != null && Story.ChaptersList.Count > 0)
            {
                _serviceStory.SetCurrentStory(Story);

                var result = await _servicePopup.ShowPopupAsync<Views.Popup.PopupChaptersPage, ViewModels.PopupViewModels.PopupChaptersPageViewModel, Story>(
                    onPresenting: vm => vm.OnNavigate(Story));

                if (result.WasDismissedByTappingOutsideOfPopup)
                {
                    return;
                }

                if (result.Result != null)
                {
                    _serviceStory.SetCurrentStory(result.Result);
                    this.InitStory();
                }
            }
        }

        private async void ExecuteDisplayStoryInfoCommand()
        {
            await ServiceNavigation.NavigateTo<Views.StoryInfoPage>();
        }

        /// <summary>
        /// Gets the Story from either the cache if exists, or from the web if it does not.
        /// And displays it, or error.
        /// </summary>
        private async void InitStory()
        {
            try
            {
                ViewState = ViewStateEnum.Loading;

                var currentStory = _serviceStory.GetCurrentStory()
                    ?? throw new Exception("Story must not be null.");

                var alreadyLoadedStory = _serviceStory.GetAlreadyLoadedStory(currentStory);
                if (alreadyLoadedStory != null)
                {
                    // Get story from cache
                    Story = alreadyLoadedStory;
                }
                else
                {
                    // or from web
                    Story = await _serviceStory.InitStory();
                }

                if (Story == null)
                {
                    throw new Exception("Story must not be null.");
                }

                Title = Story.Title;

                _serviceStory.AddAlreadyLoadedStory(Story);

                ViewState = Helpers.ViewStateEnum.Display;
            }
            catch (Exception ex)
            {
                XStory.Logger.ServiceLog.Error(ex);
                ViewState = Helpers.ViewStateEnum.Error;
            }
        }

        private void ExecuteOpenStoryActionsCommand()
        {
            // TODO: command bound nowhere ; PopupStoryActionsPage is a popup, not a page, so it cannot be pushed (it was already not working before)
        }

        private async void ExecuteOpenInBrowserCommand()
        {
            Uri storyUri = new Uri(Story.Url);

            await Browser.Default.OpenAsync(storyUri, BrowserLaunchMode.External);
        }

        private void ExecuteToggleStoryInfosCommand()
        {
            if (!IsStoryInfoVisible)
            {
                IsStoryInfoVisible = true;
            }
            else
            {
                IsStoryInfoVisible = false;
            }
        }
    }
}
