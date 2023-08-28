using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using XStory.DTO;
using XStory.Helpers;
using XStory.Logger;

namespace XStory.ViewModels
{
	public class AuthorPageViewModel : BaseViewModel
	{
		#region --- Fields ---

		BL.Web.DSLocator.Contracts.IServiceAuthor _serviceAuthor;

		private Author _author;
		public Author Author
		{
			get { return _author; }
			set { SetProperty(ref _author, value); }
		}

		public DelegateCommand<DTO.Story> AuthorStoryItemTappedCommand { get; set; }

		#endregion

		#region --- Ctor ---
		public AuthorPageViewModel(INavigationService navigationService, XStory.BL.Web.DSLocator.Contracts.IServiceAuthor serviceAuthor) : base(navigationService)
		{
			_serviceAuthor = serviceAuthor;

			AuthorStoryItemTappedCommand = new DelegateCommand<DTO.Story>((story) => ExecuteAuthorStoryItemTappedCommand(story));

			ViewState = Helpers.ViewStateEnum.Loading;
		}
		#endregion

		private async void ExecuteAuthorStoryItemTappedCommand(Story story)
		{
			var navigationParams = new NavigationParameters();

			if (story.ChaptersList != null && story.ChaptersList.Count > 0)
			{
				// if multi sub chapters
				navigationParams.Add("story", story);
				await NavigationService.NavigateAsync(nameof(Views.Popup.PopupChaptersPage), navigationParams);
			}
			else
			{
				// only one chapter
				navigationParams.Add("storyUrl", story.Url);
				await NavigationService.NavigateAsync(nameof(Views.StoryPage), navigationParams);
			}
		}

		private async void InitAuthor(Author author)
		{
			try
			{
				if (author == null)
				{
					ViewState = Helpers.ViewStateEnum.Error;
				}

				Title = author.Name;

				author = await _serviceAuthor.GetAuthorPage(StaticContext.DATASOURCE, author);
				Author = author;

				ViewState = Helpers.ViewStateEnum.Display;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				ViewState = Helpers.ViewStateEnum.Error;
			}
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			if (parameters.ContainsKey("author"))
			{
				var author = parameters.GetValue<DTO.Author>("author");

				InitAuthor(author);
			}

			if (parameters.ContainsKey("selectedChapter"))
			{
				var chapter = parameters.GetValue<DTO.Story>("selectedChapter");
				if (chapter != null)
				{
					this.GoToChapter(chapter);
				}
			}
		}

		private async void GoToChapter(DTO.Story chapter)
		{
			string chapterUrl = chapter.Url;

			var navigationParams = new NavigationParameters()
			{
				{ "storyUrl", chapterUrl }
			};
			await NavigationService.NavigateAsync(nameof(Views.StoryPage), navigationParams);
		}
	}
}
