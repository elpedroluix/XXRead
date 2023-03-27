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
    public class PopupChaptersPageViewModel : BaseViewModel
    {
        #region --- Fields ---
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

        public DelegateCommand ClosePopupCommand { get; set; }
        public DelegateCommand<DTO.Story> ChapterTappedCommand { get; set; }
        #endregion

        public PopupChaptersPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //_serviceCategorySQLite = serviceCategorySQLite;

            ClosePopupCommand = new DelegateCommand(ExecuteClosePopupCommand);
            ChapterTappedCommand = new DelegateCommand<DTO.Story>((chapter) => ExecuteChapterTappedCommand(chapter));
        }

        private void ExecuteChapterTappedCommand(Story chapter)
        {
            _selectedChapter = chapter;

            this.ClosePopupCommand.Execute();
        }

        private async void ExecuteClosePopupCommand()
        {
            var navigationParams = new NavigationParameters();
            if (_selectedChapter != null)
            {
                navigationParams.Add("selectedChapter", _selectedChapter);
            };

            await NavigationService.GoBackAsync(navigationParams);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                Story story = parameters.GetValue<Story>("story");

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
