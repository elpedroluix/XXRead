using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XStory.DTO;
using XStory.Logger;
using XXRead.Helpers;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels
{
    public class AuthorPageViewModel : BaseViewModel
    {
        #region --- Fields ---

        private XStory.BL.Common.Contracts.IServiceAuthor _serviceAuthor;
        private XStory.BL.Common.Contracts.IServiceStory _serviceStory;

        private IPopupService _popupService;

        private bool _canLoadMorePages;
        public bool CanLoadMorePages
        {
            get { return _canLoadMorePages; }
            set { SetProperty(ref _canLoadMorePages, value); }
        }

        private Author _author;
        public Author Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value); }
        }

        private ObservableCollection<Story> _authorStories;
        public ObservableCollection<Story> AuthorStories
        {
            get { return _authorStories; }
            set { SetProperty(ref _authorStories, value); }
        }

        public RelayCommand<Story> StoriesItemTappedCommand { get; set; }
        public RelayCommand LoadMoreStoriesCommand { get; set; }

        #endregion

        #region --- Ctor ---
        public AuthorPageViewModel(Prism.Navigation.INavigationService navigationService,
            IPopupService popupService,
            XStory.BL.Common.Contracts.IServiceAuthor serviceAuthor,
            XStory.BL.Common.Contracts.IServiceStory serviceStory) : base(navigationService)
        {
            _serviceAuthor = serviceAuthor;
            _serviceStory = serviceStory;

            _popupService = popupService;

            StoriesItemTappedCommand = new RelayCommand<Story>((story) => ExecuteStoriesItemTappedCommand(story));
            LoadMoreStoriesCommand = new RelayCommand(ExecuteLoadMoreStoriesCommand);

            this.InitAuthor();
        }
        #endregion

        private async void ExecuteStoriesItemTappedCommand(Story story)
        {
            if (story.ChaptersList != null && story.ChaptersList.Count > 0)
            {
                // if multi chapters
                var result = await _popupService.ShowPopupAsync<ViewModels.PopupViewModels.PopupChaptersPageViewModel>(onPresenting: vm=>vm.OnNavigate(story));
                if (result == null || result.GetType() == typeof(object))
                {
                    return;
                }
                story = result as XStory.DTO.Story;
            }

            // Problem ! Unique Story property will make story chapters confusion after the case :
            // StoryPage (Story X-ChapterXX) -> Click AuthorPage -> Click Story -> StoryPage (StoryY-ChapterYY)
            // Back to AuthorPage -> Back to StoryPage (StoryX-ChapterXX)
            // Click Next/Previous Story = StoryPage (StoryY-ChapterYX) ==> Unwanted scenario
            
            _serviceStory.SetCurrentStory(story);
            await NavigationService.NavigateAsync(nameof(Views.StoryPage));
        }

        private async void ExecuteLoadMoreStoriesCommand()
        {
            // Get next page stories
            _author = await _serviceAuthor.GetAuthorStoriesNextPage(_author);

            // Display / hide button "Load more stories"
            CanLoadMorePages = _author.HasMorePages;

            AuthorStories = new ObservableCollection<Story>(_author.Stories);
        }

        private async void InitAuthor()
        {
            try
            {
                ViewState = ViewStateEnum.Loading;

                Author currentAuthor = _serviceAuthor.GetCurrentAuthor();
                if (currentAuthor == null)
                {
                    throw new Exception("Author must not be null.");
                }

                Title = currentAuthor.Name;

                var alreadyLoadedAuthor = _serviceAuthor.GetAlreadyLoadedAuthor(currentAuthor);
                if (alreadyLoadedAuthor != null)
                {
                    Author = alreadyLoadedAuthor;
                }
                else
                {
                    Author = await _serviceAuthor.InitAuthor();
                }

                if (Author == null)
                {
                    throw new Exception("Story must not be null.");
                }

                _serviceAuthor.AddAlreadyLoadedAuthor(Author);

                AuthorStories = new ObservableCollection<Story>(_author.Stories);

                CanLoadMorePages = _author.HasMorePages;

                ViewState = ViewStateEnum.Display;
            }
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                ViewState = Helpers.ViewStateEnum.Error;
            }
        }

        /*public override void OnNavigatedTo(INavigationParameters parameters)
		{
			if (Author != null && Author != _serviceAuthor.GetCurrentAuthor())
			{
				this.InitAuthor();
			}
		}*/
    }
}
