using Prism.Commands;
using Prism.Navigation;
using System;
using System.Linq;
using Xamarin.Essentials;
using XStory.DTO;
using XStory.Helpers;

namespace XStory.ViewModels
{
	public class StoryPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private BL.Web.DSLocator.Contracts.IServiceStory _serviceStory;
		private BL.Common.Contracts.IServiceStory _elServiceStory;
		private BL.Common.Contracts.IServiceAuthor _elServiceAuthor;

		private bool _isStoryInfoVisible;
		public bool IsStoryInfoVisible
		{
			get { return _isStoryInfoVisible; }
			set { SetProperty(ref _isStoryInfoVisible, value); }
		}

		private Story _story;
		public Story Story
		{
			get { return _story; }
			set { SetProperty(ref _story, value); }
		}

		public DelegateCommand AuthorTappedCommand { get; set; }
		public DelegateCommand<string> ChapterArrowTapped { get; set; }
		public DelegateCommand ChapterNameTappedCommand { get; set; }
		public DelegateCommand DisplayStoryInfoCommand { get; set; }
		public DelegateCommand ShareStoryCommand { get; set; }
		public DelegateCommand ToggleStoryInfosCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public StoryPageViewModel(INavigationService navigationService, BL.Web.DSLocator.Contracts.IServiceStory serviceStory,
			BL.Common.Contracts.IServiceStory elServiceStory,
			BL.Common.Contracts.IServiceAuthor elServiceAuthor)
			: base(navigationService)
		{
			_serviceStory = serviceStory;

			_elServiceStory = elServiceStory;
			_elServiceAuthor = elServiceAuthor;

			AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
			AuthorTappedCommand = new DelegateCommand(ExecuteAuthorTappedCommand);
			ChapterArrowTapped = new DelegateCommand<string>((direction) => ExecuteChapterArrowTappedCommand(direction));
			ChapterNameTappedCommand = new DelegateCommand(ExecuteChapterNameTappedCommand);
			DisplayStoryInfoCommand = new DelegateCommand(ExecuteDisplayStoryInfoCommand);
			ShareStoryCommand = new DelegateCommand(ExecuteShareStoryCommand);
			ToggleStoryInfosCommand = new DelegateCommand(ExecuteToggleStoryInfosCommand);
			TryAgainCommand = new DelegateCommand(InitStory);

			this.InitStory();
		}

		#endregion

		private async void ExecuteAuthorTappedCommand()
		{
			if (Story.Author != null)
			{
				_elServiceAuthor.SetCurrentAuthor(Story.Author);

				await NavigationService.NavigateAsync(nameof(Views.AuthorPage), new NavigationParameters());
			}
		}

		private void ExecuteChapterArrowTappedCommand(string direction)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(direction)) throw new ArgumentNullException();

				Story chapter = null;

				switch (direction)
				{
					case "left":
						int previousChapter = Story.ChapterNumber - 1;
						chapter = Story.ChaptersList.ElementAt(previousChapter - 1);
						break;

					case "right":
						int nextChapter = Story.ChapterNumber + 1;
						chapter = Story.ChaptersList.ElementAt(nextChapter - 1);
						break;
				}

				if (chapter != null)
				{
					_elServiceStory.SetCurrentStory(chapter);

					ViewState = Helpers.ViewStateEnum.Loading;

					this.InitStory();
				}
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
			}
		}

		private async void ExecuteChapterNameTappedCommand()
		{
			if (Story != null && Story.ChaptersList != null && Story.ChaptersList.Count > 0)
			{
				_elServiceStory.SetCurrentStory(Story);

				await NavigationService.NavigateAsync(nameof(Views.Popup.PopupChaptersPage));
			}
		}

		private async void ExecuteDisplayStoryInfoCommand()
		{
			await NavigationService.NavigateAsync(nameof(Views.StoryInfoPage));
		}

		/// <summary>
		/// Gets the Story from either the cache if exists, or from the web if it does not.
		/// And displays it, or error.
		/// </summary>
		private async void InitStory()
		{
			try
			{
				ViewState = ViewStateEnum.Loading;

				var currentStory = _elServiceStory.GetCurrentStory();
				if (currentStory == null)
				{
					throw new Exception("Story must not be null.");
				}

				var alreadyLoadedStory = _elServiceStory.GetAlreadyLoadedStory(currentStory);
				if (alreadyLoadedStory != null)
				{
					// Get story from cache
					Story = alreadyLoadedStory;
				}
				else
				{
					// or from web
					Story = await _elServiceStory.InitStory();
				}

				if (Story == null)
				{
					throw new Exception("Story must not be null.");
				}

				Title = Story.Title;

				_elServiceStory.AddAlreadyLoadedStory(Story);

				ViewState = Helpers.ViewStateEnum.Display;
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
				ViewState = Helpers.ViewStateEnum.Error;
			}
		}

		private async void ExecuteShareStoryCommand()
		{
			try
			{
				await Share.RequestAsync(new ShareTextRequest()
				{
					Uri = Story.Url,
					Text = Story.Title,
					Title = "Partager ce récit"
				});
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
			}
		}

		private void ExecuteToggleStoryInfosCommand()
		{
			if (!IsStoryInfoVisible)
			{
				IsStoryInfoVisible = true;
			}
			else
			{
				IsStoryInfoVisible = false;
			}
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			if (Story != null && Story != _elServiceStory.GetCurrentStory())
			{
				this.InitStory();
			}
		}
	}
}
