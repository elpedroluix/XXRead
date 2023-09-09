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

		private BL.Common.Contracts.IServiceAuthor _elServiceAuthor;
		private BL.Common.Contracts.IServiceStory _elServiceStory;

		private Author _author;
		public Author Author
		{
			get { return _author; }
			set { SetProperty(ref _author, value); }
		}

		public DelegateCommand<DTO.Story> AuthorStoryItemTappedCommand { get; set; }

		#endregion

		#region --- Ctor ---
		public AuthorPageViewModel(INavigationService navigationService, XStory.BL.Web.DSLocator.Contracts.IServiceAuthor serviceAuthor,
			BL.SQLite.Contracts.IServiceCategory serviceCategorySQLite,
			BL.SQLite.Contracts.IServiceStory serviceStorySQLite,
			BL.Common.Contracts.IServiceAuthor elServiceAuthor,
			BL.Common.Contracts.IServiceStory elServiceStory) : base(navigationService)
		{
			_serviceAuthor = serviceAuthor;

			_serviceStorySQLite = serviceStorySQLite;
			_serviceCategorySQLite = serviceCategorySQLite;

			_elServiceAuthor = elServiceAuthor;
			_elServiceStory = elServiceStory;

			AuthorStoryItemTappedCommand = new DelegateCommand<DTO.Story>((story) => ExecuteAuthorStoryItemTappedCommand(story));

			this.InitAuthor();
		}
		#endregion

		private async void ExecuteAuthorStoryItemTappedCommand(Story story)
		{
			_elServiceStory.SetCurrentStory(story);

			if (story.ChaptersList != null && story.ChaptersList.Count > 0)
			{
				// if multi sub chapters
				await NavigationService.NavigateAsync(nameof(Views.Popup.PopupChaptersPage));
			}
			else
			{
				// only one chapter
				await NavigationService.NavigateAsync(nameof(Views.StoryPage));
			}
		}

		private async void InitAuthor()
		{
			try
			{
				ViewState = ViewStateEnum.Loading;

				DTO.Author currentAuthor = _elServiceAuthor.GetCurrentAuthor();
				if (currentAuthor == null)
				{
					throw new Exception("Author must not be null.");
				}

				Title = currentAuthor.Name;

				var alreadyLoadedAuthor = _elServiceAuthor.GetAlreadyLoadedAuthor(currentAuthor);
				if (alreadyLoadedAuthor != null)
				{
					Author = alreadyLoadedAuthor;
				}
				else
				{
					Author = await _elServiceAuthor.InitAuthor();
				}

				if (Author == null)
				{
					throw new Exception("Story must not be null.");
				}

				_elServiceAuthor.AddAlreadyLoadedAuthor(Author);

				ViewState = ViewStateEnum.Display;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				ViewState = Helpers.ViewStateEnum.Error;
			}
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			if (Author != null && Author != _elServiceAuthor.GetCurrentAuthor())
			{
				this.InitAuthor();
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
