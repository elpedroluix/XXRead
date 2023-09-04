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

		private BL.Web.DSLocator.Contracts.IServiceAuthor _serviceAuthor;

		private BL.SQLite.Contracts.IServiceStory _serviceStorySQLite;
		private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

		private Author _author;
		public Author Author
		{
			get { return _author; }
			set { SetProperty(ref _author, value); }
		}

		public DelegateCommand<DTO.Story> AuthorStoryItemTappedCommand { get; set; }

		#endregion

		#region --- Ctor ---
		public AuthorPageViewModel(INavigationService navigationService, XStory.BL.Web.DSLocator.Contracts.IServiceAuthor serviceAuthor
			, BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite
			, BL.SQLite.Contracts.IServiceStory serviceStorySQLite) : base(navigationService)
		{
			_serviceAuthor = serviceAuthor;

			_serviceStorySQLite = serviceStorySQLite;
			_serviceCategorySQLite = serviceCategorySQLite;

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

				// BEGIN StaticContext cache
				var alreadyLoadedAuthor = StaticContext.ListAlreadyLoadedAuthors.FirstOrDefault(aauthor => aauthor.Url.Contains(author.Url));
				if (alreadyLoadedAuthor != null)
				{
					author = alreadyLoadedAuthor;
				}
				else
				{
					author = await _serviceAuthor.GetAuthorPage(StaticContext.DATASOURCE, author);
				}

				Author = author;

				if (Author != null)
				{
					if (alreadyLoadedAuthor == null && StaticContext.ListAlreadyLoadedAuthors.FirstOrDefault(aauthor => aauthor.Url.Contains(author.Url)) == null)
					{
						// If Author does not exists in StaticContext.ListAlreadyLoadedAuthors -> add in cache
						StaticContext.ListAlreadyLoadedAuthors.Add(Author);

						// END StaticContext cache
					}
					ViewState = Helpers.ViewStateEnum.Display;
				}
				else
				{
					ViewState = Helpers.ViewStateEnum.Error;
				}
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
