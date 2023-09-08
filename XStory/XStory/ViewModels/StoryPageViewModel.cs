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

		string storyUrl = string.Empty;
		#endregion

		#region --- Ctor ---
		public StoryPageViewModel(INavigationService navigationService, BL.Web.DSLocator.Contracts.IServiceStory serviceStory,
			BL.Common.Contracts.IServiceStory elServiceStory)
			: base(navigationService)
		{
			_serviceStory = serviceStory;
			_elServiceStory = elServiceStory;

			ViewState = Helpers.ViewStateEnum.Loading;

			AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
			AuthorTappedCommand = new DelegateCommand(ExecuteAuthorTappedCommand);
			ChapterArrowTapped = new DelegateCommand<string>((direction) => ExecuteChapterArrowTappedCommand(direction));
			ChapterNameTappedCommand = new DelegateCommand(ExecuteChapterNameTappedCommand);
			DisplayStoryInfoCommand = new DelegateCommand(ExecuteDisplayStoryInfoCommand);
			ShareStoryCommand = new DelegateCommand(ExecuteShareStoryCommand);
			ToggleStoryInfosCommand = new DelegateCommand(ExecuteToggleStoryInfosCommand);
			TryAgainCommand = new DelegateCommand(InitStory);
		}

		#endregion

		private async void ExecuteAuthorTappedCommand()
		{
			if (Story.Author != null && !string.IsNullOrWhiteSpace(Story.Author.Id))
			{
				var navigationParams = new NavigationParameters()
				{
					{ "author", Story.Author }
				};

				await NavigationService.NavigateAsync(nameof(Views.AuthorPage), navigationParams);
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
					// TODO : optimiser !!!
					storyUrl = chapter.Url;
					Story = chapter;

					ViewState = Helpers.ViewStateEnum.Loading;

					InitStory();
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
				var navigationParams = new NavigationParameters()
				{
					{ "story", Story }
				};

				await NavigationService.NavigateAsync(nameof(Views.Popup.PopupChaptersPage), navigationParams);
			}
		}

		private async void ExecuteDisplayStoryInfoCommand()
		{
			INavigationParameters navigationParameters = new NavigationParameters()
			{
				{ "story" , Story }
			};

			await NavigationService.NavigateAsync(nameof(Views.StoryInfoPage), navigationParameters);
		}

		/// <summary>
		/// Gets the Story from either the cache if exists, or from the web if it does not.
		/// </summary>
		private async void InitStory()
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(storyUrl))
				{
					ViewState = ViewStateEnum.Loading;

					var alreadyLoadedStory = StaticContext.ListAlreadyLoadedStories.FirstOrDefault(story => story.Url.Contains(storyUrl));
					if (alreadyLoadedStory != null)
					{
						Story = alreadyLoadedStory;
					}
					else
					{
						Story = await _serviceStory.GetStory(StaticContext.DATASOURCE, storyUrl);
					}

					if (Story != null)
					{
						Title = Story.Title;
						if (alreadyLoadedStory == null && StaticContext.ListAlreadyLoadedStories.FirstOrDefault(story => story.Url.Contains(storyUrl)) == null)
						{
							// If Story does not exists in StaticContext.ListAlreadyLoadedStories -> add in cache
							StaticContext.ListAlreadyLoadedStories.Add(Story);
						}
						ViewState = Helpers.ViewStateEnum.Display;
					}
					else
					{
						ViewState = Helpers.ViewStateEnum.Error;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
				ViewState = Helpers.ViewStateEnum.Error;
			}

		}

		protected override void ExecuteAppearingCommand()
		{
			// InitStory is called here because of property storyUrl initialized during OnNavigatedTo
			this.InitStory();

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
				return;
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
			try
			{
				if (string.IsNullOrWhiteSpace(storyUrl))
				{
					storyUrl = parameters.GetValue<string>("storyUrl");
				}
				else if (parameters.ContainsKey("selectedChapter"))
				{
					var story = parameters.GetValue<Story>("selectedChapter");
					storyUrl = story.Url;

					this.InitStory();
				}
			}
			catch (Exception e)
			{
				storyUrl = null;
			}

		}
	}
}
