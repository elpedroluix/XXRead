using CommunityToolkit.Mvvm.Input;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.UI.Constants;
using XXRead.UI.Dialogs;
using XXRead.UI.Platform;
using XXRead.UI.Services;

namespace XXRead.UI.ViewModels
{
	/// <summary>
	/// Copied from the XAML UI's StoryPageViewModel and BaseStoryViewModel.
	/// Transient : the page always displays the current story of the session.
	/// </summary>
	public class StoryPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private IServiceStory _serviceStory;
		private IServiceAuthor _serviceAuthor;
		private IServiceDialog _serviceDialog;
		private IServicePlatform _servicePlatform;

		private Story _story;
		public Story Story
		{
			get { return _story; }
			set { SetProperty(ref _story, value); }
		}

		private string _saveButtonText;
		public string SaveButtonText
		{
			get { return _saveButtonText; }
			set { SetProperty(ref _saveButtonText, value); }
		}

		private bool _isStoryInfoVisible;
		public bool IsStoryInfoVisible
		{
			get { return _isStoryInfoVisible; }
			set { SetProperty(ref _isStoryInfoVisible, value); }
		}

		public bool HasPreviousChapter => GetChapter(-1) != null;

		public bool HasNextChapter => GetChapter(1) != null;
		#endregion

		#region --- Commands ---
		public RelayCommand AuthorTappedCommand { get; set; }
		public RelayCommand PreviousChapterCommand { get; set; }
		public RelayCommand NextChapterCommand { get; set; }
		public RelayCommand ChapterNameTappedCommand { get; set; }
		public RelayCommand SaveStoryCommand { get; set; }
		public RelayCommand ShareStoryCommand { get; set; }
		public RelayCommand OpenInBrowserCommand { get; set; }
		public RelayCommand ToggleStoryInfosCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public StoryPageViewModel(IServiceNavigation serviceNavigation,
			IServiceDialog serviceDialog,
			IServicePlatform servicePlatform,
			IServiceStory serviceStory,
			IServiceAuthor serviceAuthor) : base(serviceNavigation)
		{
			_serviceStory = serviceStory;
			_serviceAuthor = serviceAuthor;
			_serviceDialog = serviceDialog;
			_servicePlatform = servicePlatform;

			ViewState = ViewStateEnum.Loading;

			AuthorTappedCommand = new RelayCommand(ExecuteAuthorTappedCommand);
			PreviousChapterCommand = new RelayCommand(() => ChangeChapter(GetChapter(-1)));
			NextChapterCommand = new RelayCommand(() => ChangeChapter(GetChapter(1)));
			ChapterNameTappedCommand = new RelayCommand(ExecuteChapterNameTappedCommand);
			SaveStoryCommand = new RelayCommand(ExecuteSaveStoryCommand);
			ShareStoryCommand = new RelayCommand(ExecuteShareStoryCommand);
			OpenInBrowserCommand = new RelayCommand(ExecuteOpenInBrowserCommand);
			ToggleStoryInfosCommand = new RelayCommand(() => IsStoryInfoVisible = !IsStoryInfoVisible);
			TryAgainCommand = new RelayCommand(ExecuteTryAgainCommand);
		}
		#endregion

		public override async Task OnAppearing()
		{
			await InitStory();
		}

		private void ExecuteAuthorTappedCommand()
		{
			if (Story?.Author == null)
			{
				return;
			}

			_serviceAuthor.SetCurrentAuthor(Story.Author);

			// Navigation to the author page : step 8.4 (AuthorPage port)
		}

		/// <summary>
		/// Chapter next to the current one : -1 = previous, 1 = next. Null when there is none.
		/// </summary>
		private Story GetChapter(int offset)
		{
			if (Story?.ChaptersList == null)
			{
				return null;
			}

			return Story.ChaptersList.ElementAtOrDefault(Story.ChapterNumber - 1 + offset);
		}

		private async void ChangeChapter(Story chapter)
		{
			if (chapter == null)
			{
				return;
			}

			_serviceStory.SetCurrentStory(chapter);

			await InitStory();
		}

		private async void ExecuteChapterNameTappedCommand()
		{
			if (Story?.ChaptersList == null || Story.ChaptersList.Count == 0)
			{
				return;
			}

			var parameters = new Dictionary<string, object>() { { nameof(DialogChapters.Story), Story } };
			Story chapter = await _serviceDialog.ShowAsync<DialogChapters, Story>(parameters);

			ChangeChapter(chapter);
		}

		/// <summary>
		/// Gets the Story from either the cache if exists, or from the web if it does not.
		/// And displays it, or error.
		/// </summary>
		private async Task InitStory()
		{
			try
			{
				ViewState = ViewStateEnum.Loading;

				Story currentStory = _serviceStory.GetCurrentStory();
				if (currentStory == null)
				{
					throw new Exception("Story must not be null.");
				}

				// Title of the list item while the story is loading
				Title = currentStory.Title;

				Story story = _serviceStory.GetAlreadyLoadedStory(currentStory) ?? await _serviceStory.InitStory();
				if (story == null)
				{
					throw new Exception("Story must not be null.");
				}

				_serviceStory.AddAlreadyLoadedStory(story);
				await SetSaveButtonText(story);

				Story = story;
				Title = story.Title;
				ViewState = ViewStateEnum.Display;
			}
			catch (Exception ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
				ViewState = ViewStateEnum.Error;
			}
		}

		private async Task SetSaveButtonText(Story story)
		{
			SaveButtonText = await _serviceStory.GetStorySQLite(story) == null
				? StoryPageConstants.STORYPAGE_SAVE // if not saved in DB
				: StoryPageConstants.STORYPAGE_DELETE; // if saved in DB
		}

		private async void ExecuteSaveStoryCommand()
		{
			try
			{
				// TODO : always inserts, even when the button says "Supprimer" : a saved story cannot be removed (same in the XAML UI)
				await _serviceStory.InsertStorySQLite(Story);

				await SetSaveButtonText(Story);
			}
			catch (Exception ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
			}
		}

		private async void ExecuteShareStoryCommand()
		{
			try
			{
				await _servicePlatform.Share(StoryPageConstants.STORYPAGE_SHARE_TITLE, Story.Title, Story.Url);
			}
			catch (Exception ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
			}
		}

		private async void ExecuteOpenInBrowserCommand()
		{
			try
			{
				await _servicePlatform.OpenInBrowser(Story.Url);
			}
			catch (Exception ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
			}
		}

		protected override async void ExecuteTryAgainCommand()
		{
			await InitStory();
		}
	}
}
