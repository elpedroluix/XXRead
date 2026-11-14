using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XStory.DTO;
using XXRead.Helpers;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels.PopupViewModels
{
	public class PopupChaptersPageViewModel : BasePopupViewModel
	{
		#region --- Fields ---
		private XXRead.Core.Services.IServiceStory _serviceStory;
		private IServicePopup _servicePopup;

		private Story _selectedChapter;
        private Story _currentStory;
        public Story CurrentStory
        {
            get { return _currentStory; }
            set { SetProperty(ref _currentStory, value); }
        }

        private string _storyTitle;
		public string StoryTitle
		{
			get { return _storyTitle; }
			set { SetProperty(ref _storyTitle, value); }
		}

		private ObservableCollection<Story> _chapters;
		public ObservableCollection<Story> Chapters
		{
			get { return _chapters; }
			set { SetProperty(ref _chapters, value); }
		}

		public RelayCommand<Story> ChapterTappedCommand { get; set; }
		#endregion

		public PopupChaptersPageViewModel(IServiceNavigation serviceNavigation,
			XXRead.Core.Services.IServiceStory serviceStory,
			IServicePopup servicePopup) : base(serviceNavigation)
		{
			_serviceStory = serviceStory;
			_servicePopup = servicePopup;

			ClosePopupCommand = new RelayCommand(ExecuteClosePopupCommand);
			ChapterTappedCommand = new RelayCommand<Story>((chapter) => ExecuteChapterTappedCommand(chapter));
		}

		private void ExecuteChapterTappedCommand(Story chapter)
		{
			_selectedChapter = chapter;

			this.ClosePopupCommand.Execute(null);
		}

		public override void ExecuteClosePopupCommand()
		{
			if (_selectedChapter == null)
			{
				RequestClose();
				return;
			}

			RequestClose(_selectedChapter);
		}

		private void InitChaptersList()
		{
			try
			{
				Story story = _currentStory ?? _serviceStory.GetCurrentStory();

				if (story != null && story.ChaptersList != null)
				{
					StoryTitle = story.Title;
					Chapters = new ObservableCollection<Story>(story.ChaptersList);
				}
			}
			catch (Exception ex)
			{
				XStory.Logger.ServiceLog.Error(ex);
				Chapters = null;
			}
		}

		private async void RequestClose(Story story = null)
		{
			await _servicePopup.ClosePopupAsync(story);
		}

		public void OnNavigate(Story story)
		{
			_currentStory = story;

            this.InitChaptersList();
        }
	}
}
