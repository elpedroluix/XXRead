using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using XStory.DTO;
using XStory.Helpers;

namespace XStory.ViewModels.PopupViewModels
{
	public class PopupChaptersPageViewModel : BasePopupViewModel
	{
		#region --- Fields ---
		private BL.Common.Contracts.IServiceStory _elServiceStory;

		private Story _selectedChapter;

		private string _storyTitle;
		public string StoryTitle
		{
			get { return _storyTitle; }
			set { SetProperty(ref _storyTitle, value); }
		}

		private ObservableCollection<DTO.Story> _chapters;
		public ObservableCollection<DTO.Story> Chapters
		{
			get { return _chapters; }
			set { SetProperty(ref _chapters, value); }
		}

		public DelegateCommand<DTO.Story> ChapterTappedCommand { get; set; }
		#endregion

		public PopupChaptersPageViewModel(INavigationService navigationService, BL.Common.Contracts.IServiceStory elServiceStory) : base(navigationService)
		{
			_elServiceStory = elServiceStory;

			ClosePopupCommand = new DelegateCommand(ExecuteClosePopupCommand);
			ChapterTappedCommand = new DelegateCommand<DTO.Story>((chapter) => ExecuteChapterTappedCommand(chapter));

			this.InitChaptersList();
		}

		private void ExecuteChapterTappedCommand(Story chapter)
		{
			_selectedChapter = chapter;

			this.ClosePopupCommand.Execute();
		}

		protected override async void ExecuteClosePopupCommand()
		{
			if (_selectedChapter != null)
			{
				_elServiceStory.SetCurrentStory(_selectedChapter);
			};

			await NavigationService.GoBackAsync();
		}

		private void InitChaptersList()
		{
			try
			{
				Story story = _elServiceStory.GetCurrentStory();

				if (story != null && story.ChaptersList != null)
				{
					StoryTitle = story.Title;
					Chapters = new ObservableCollection<DTO.Story>(story.ChaptersList);
				}
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
				Chapters = null;
			}
		}
	}
}
