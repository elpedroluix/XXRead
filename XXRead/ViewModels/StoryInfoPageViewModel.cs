using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using XXRead.Core.Models;
using XXRead.Helpers.Services;

namespace XXRead.ViewModels
{
	public class StoryInfoPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private Story _story;
		public Story Story
		{
			get { return _story; }
			set { SetProperty(ref _story, value); }
		}

		private bool _isChapterListVisible;
		public bool IsChapterListVisible
		{
			get { return _isChapterListVisible; }
			set { SetProperty(ref _isChapterListVisible, value); }
		}
		#endregion

		#region --- Commands ---
		public RelayCommand<string> ChapterSelectionCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public StoryInfoPageViewModel(IServiceNavigation serviceNavigation) : base(serviceNavigation)
		{
			IsChapterListVisible = true;

			// TODO: bug - Story is never set (it was done in the commented OnNavigatedTo), the page is displayed empty
			ChapterSelectionCommand = new RelayCommand<string>((url) => ExecuteChapterSelectionCommand(url));
		} 
		#endregion

		private async void ExecuteChapterSelectionCommand(string url)
		{
			// TODO: bug - the selected chapter (url) is never passed to StoryPage, which displays the current story
			await ServiceNavigation.NavigateTo<Views.StoryPage>();
		}
	}
}
