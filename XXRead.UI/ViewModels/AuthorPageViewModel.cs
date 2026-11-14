using CommunityToolkit.Mvvm.Input;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.UI.Dialogs;
using XXRead.UI.Services;

namespace XXRead.UI.ViewModels
{
	/// <summary>
	/// Copied from the XAML UI's AuthorPageViewModel.
	/// Scoped : back from one of the author's stories, the page is displayed as the user left it (tab, scroll position).
	/// It is loaded again when the current author of the session changes.
	/// </summary>
	public class AuthorPageViewModel : BaseViewModel
	{
		public const int TAB_INFOS = 0;
		public const int TAB_STORIES = 1;

		#region --- Fields ---
		private IServiceAuthor _serviceAuthor;
		private IServiceStory _serviceStory;
		private IServiceDialog _serviceDialog;

		private Author _author;
		public Author Author
		{
			get { return _author; }
			set { SetProperty(ref _author, value); }
		}

		private List<Story> _authorStories;
		/// <summary>
		/// New list on each page loaded : the source completes the same Author instance, Author alone would not notify.
		/// </summary>
		public List<Story> AuthorStories
		{
			get { return _authorStories; }
			set { SetProperty(ref _authorStories, value); }
		}

		private bool _canLoadMorePages;
		public bool CanLoadMorePages
		{
			get { return _canLoadMorePages; }
			set { SetProperty(ref _canLoadMorePages, value); }
		}

		private int _selectedTab;
		/// <summary>
		/// <see cref="TAB_INFOS"/> or <see cref="TAB_STORIES"/>.
		/// </summary>
		public int SelectedTab
		{
			get { return _selectedTab; }
			set { SetProperty(ref _selectedTab, value); }
		}

		/// <summary>
		/// Scroll position of the stories list when a story is opened, restored by the page when the user comes back.
		/// </summary>
		public double ScrollPosition { get; set; }
		#endregion

		#region --- Commands ---
		public RelayCommand<Story> StoriesItemTappedCommand { get; set; }
		/// <summary>
		/// Async : no concurrent execution, a double tap does not skip a page.
		/// </summary>
		public AsyncRelayCommand LoadMoreStoriesCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public AuthorPageViewModel(IServiceNavigation serviceNavigation,
			IServiceDialog serviceDialog,
			IServiceAuthor serviceAuthor,
			IServiceStory serviceStory) : base(serviceNavigation)
		{
			_serviceAuthor = serviceAuthor;
			_serviceStory = serviceStory;
			_serviceDialog = serviceDialog;

			ViewState = ViewStateEnum.Loading;

			StoriesItemTappedCommand = new RelayCommand<Story>(ExecuteStoriesItemTappedCommand);
			LoadMoreStoriesCommand = new AsyncRelayCommand(ExecuteLoadMoreStoriesCommand);
			TryAgainCommand = new RelayCommand(ExecuteTryAgainCommand);
		}
		#endregion

		public override async Task OnAppearing()
		{
			Author currentAuthor = _serviceAuthor.GetCurrentAuthor();
			if (Author != null && Author.Url == currentAuthor?.Url)
			{
				return;
			}

			Author = null;
			AuthorStories = null;
			SelectedTab = TAB_INFOS;
			ScrollPosition = 0;

			await InitAuthor();
		}

		/// <summary>
		/// Opens the story, after the chapter selection when it has several chapters.
		/// </summary>
		private async void ExecuteStoriesItemTappedCommand(Story story)
		{
			if (story == null)
			{
				return;
			}

			if (story.ChaptersList != null && story.ChaptersList.Count > 0)
			{
				var parameters = new Dictionary<string, object>() { { nameof(DialogChapters.Story), story } };
				story = await _serviceDialog.ShowAsync<DialogChapters, Story>(parameters);
				if (story == null)
				{
					return;
				}
			}

			_serviceStory.SetCurrentStory(story);

			// TODO : navigation stack (fixed in step 8.8). The StoryPage starts again from the current story of the session :
			// Story 1 -> Author -> Story 2 -> back -> Author -> back displays Story 2 instead of Story 1
			await ServiceNavigation.NavigateTo<Pages.StoryPage>();
		}

		private async Task ExecuteLoadMoreStoriesCommand()
		{
			// On error, the author comes back as is
			Author author = await _serviceAuthor.GetAuthorStoriesNextPage(Author);

			SetAuthor(author);
		}

		/// <summary>
		/// Gets the Author from either the cache if exists, or from the web if it does not.
		/// And displays it, or error.
		/// </summary>
		private async Task InitAuthor()
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

				Author author = _serviceAuthor.GetAlreadyLoadedAuthor(currentAuthor) ?? await _serviceAuthor.InitAuthor();
				if (author == null)
				{
					throw new Exception("Author must not be null.");
				}

				_serviceAuthor.AddAlreadyLoadedAuthor(author);

				SetAuthor(author);
				ViewState = ViewStateEnum.Display;
			}
			catch (Exception ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
				ViewState = ViewStateEnum.Error;
			}
		}

		private void SetAuthor(Author author)
		{
			Author = author;
			AuthorStories = new List<Story>(author.Stories);
			// HDS only : the author's stories are paginated
			CanLoadMorePages = author.HasMorePages;
		}

		protected override async void ExecuteTryAgainCommand()
		{
			await InitAuthor();
		}
	}
}
