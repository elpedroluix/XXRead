using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XStory.DTO;
using XStory.Helpers;
using XStory.Helpers.Constants;

namespace XStory.ViewModels
{
	public class MainPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private IPageDialogService _pageDialogService;

		private BL.Common.Contracts.IServiceStory _elServiceStory;
		private BL.Common.Contracts.IServiceCategory _elServiceCategory;

		private ObservableCollection<Story> _stories;
		//private BL.Web.XStory.Contracts.IServiceStory _serviceStory;
		private BL.Web.DSLocator.Contracts.IServiceCategory _serviceCategoryWeb;

		private BL.Web.DSLocator.Contracts.IServiceStory _dsServiceStory;

		private BL.SQLite.Contracts.IServiceCategory _serviceCategorySQLite;

		private int _pageNumber;
		private List<string> _hiddenCategories;

		public ObservableCollection<Story> Stories
		{
			get { return _stories; }
			set { SetProperty(ref _stories, value); }
		}

		private bool _isStoriesListRefreshing;
		public bool IsStoriesListRefreshing
		{
			get { return _isStoriesListRefreshing; }
			set { SetProperty(ref _isStoriesListRefreshing, value); }
		}

		private Category _currentCategory;
		public Category CurrentCategory
		{
			get { return _currentCategory; }
			set
			{
				SetProperty(ref _currentCategory, value);
				// OnCurrentCategoryChanged();
			}
		}

		#endregion

		#region --- Commands ---
		public DelegateCommand CategoryTappedCommand { get; set; }
		public DelegateCommand LoadMoreStoriesCommand { get; set; }
		public DelegateCommand<DTO.Story> StoriesItemTappedCommand { get; set; }
		public DelegateCommand StoriesRefreshCommand { get; set; }
		public DelegateCommand SettingsCommand { get; set; }
		#endregion

		public MainPageViewModel(INavigationService navigationService,
			IPageDialogService pageDialogService,
			BL.Common.Contracts.IServiceStory elServiceStory,
			BL.Common.Contracts.IServiceCategory elServiceCategory)
			: base(navigationService)
		{
			_pageDialogService = pageDialogService;

			_elServiceStory = elServiceStory;
			_elServiceCategory = elServiceCategory;

			Title = MainPageConstants.MAINPAGE_TITLE;
			ViewState = ViewStateEnum.Loading;

			AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
			CategoryTappedCommand = new DelegateCommand(ExecuteCategoryTappedCommand);
			LoadMoreStoriesCommand = new DelegateCommand(ExecuteLoadMoreStoriesCommand);
			SettingsCommand = new DelegateCommand(ExecuteSettingsCommand);
			StoriesItemTappedCommand = new DelegateCommand<DTO.Story>((story) => ExecuteStoriesItemTappedCommand(story));
			StoriesRefreshCommand = new DelegateCommand(ExecuteStoriesRefreshCommand);
			TryAgainCommand = new DelegateCommand(ExecuteTryAgainCommand);

			_pageNumber = 1;
			_hiddenCategories = new List<string>();

			if (AppSettings.FirstRun)
			{
				// If FIRST run : diclaimer Message "Welcome" + "disabled categories"
				this.DisplayFirstRunMessage();

				AppSettings.FirstRun = false;
			}

			InitCategories()
				.ContinueWith(result =>
				{
					if (result.Status == TaskStatus.RanToCompletion)
					{
						InitHiddenCategories().ContinueWith(res =>
						{
							if (res.Status == TaskStatus.RanToCompletion)
							{
								InitStories();
							}
						});
					}
				});
		}

		private void DisplayFirstRunMessage()
		{
			// SUPER UGLY !
			// SUPER UGLY !
			// What about a beautiful welcome page :)
			// SUPER UGLY !
			// SUPER UGLY !
			Device.BeginInvokeOnMainThread(async () =>
			{
				await _pageDialogService.DisplayAlertAsync(
					MainPageConstants.MAINPAGE_FIRST_RUN_TITLE,
					MainPageConstants.MAINPAGE_FIRST_RUN_MESSAGE,
					"OK");
			});
		}

		/// <summary>
		/// <br>First appearing.</br>
		/// <br>Init Color themes.</br>
		/// </summary>
		protected override void ExecuteAppearingCommand()
		{
			// Have to call InitTheming() everytime VM appears because of this stupid Android BackButton issue
			InitTheming();

			this.InitStories();
		}

		/// <summary>
		/// Display Categories selection popup
		/// </summary>
		private async void ExecuteCategoryTappedCommand()
		{

			await NavigationService.NavigateAsync(nameof(Views.Popup.PopupSelectCategoryPage));
		}

		private async void ExecuteSettingsCommand()
		{
			await NavigationService.NavigateAsync(nameof(Views.SettingsPage));
		}

		private async void ExecuteStoriesItemTappedCommand(DTO.Story story)
		{
			if (story != null)
			{
				_elServiceStory.SetCurrentStory(story);
			}

			await NavigationService.NavigateAsync(nameof(Views.StoryPage));
		}

		/// <summary>
		/// When the PullToRefresh command is triggered on StoriesList.
		/// If the 1st item of the two lists is different : refresh. 
		/// Else : nothing
		/// </summary>
		private async void ExecuteStoriesRefreshCommand()
		{
			try
			{
				if (Stories == null || Stories.Count == 0)
				{
					Stories = new ObservableCollection<Story>(await _elServiceStory.InitStories());
					return;
					// OU Exception ?
				}
				IsStoriesListRefreshing = true;

				List<DTO.Story> refreshList = await _elServiceStory.RefreshStories(Stories.First());
				if (refreshList == null)
				{
					throw new Exception("Couldn't refresh stories");
				}
				else if (refreshList.Count > 0)
				{
					Stories = new ObservableCollection<Story>(refreshList);
				}

				IsStoriesListRefreshing = false;
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
				IsStoriesListRefreshing = false;
			}
		}

		private async void ExecuteLoadMoreStoriesCommand()
		{
			var moreStories = await _elServiceStory.LoadMoreStories();

			moreStories.ForEach(story => _stories.Add(story));

			Stories = new ObservableCollection<Story>(_elServiceStory.DistinctStories(_stories.ToList()));
		}

		/// <summary>
		/// Get stories on Main Page.
		/// </summary>
		/// <param name="forceInit">Force (re)init if true.</param>
		private async void InitStories(bool forceInit = false)
		{
			if (AppSettings.DataSourceChanged || AppSettings.HiddenCategoriesChanged)
			{
				forceInit = true;
				_elServiceStory.ResetPageNumber();
			}

			if (forceInit || (Stories == null || Stories.Count == 0))
			{
				ViewState = ViewStateEnum.Loading;
				try
				{
					Stories = new ObservableCollection<Story>(await _elServiceStory.InitStories());
					ViewState = ViewStateEnum.Display;
				}
				catch (Exception ex)
				{
					Logger.ServiceLog.Error(ex);
					ViewState = ViewStateEnum.Error;
				}
				AppSettings.DataSourceChanged = false;
				AppSettings.HiddenCategoriesChanged = false;
			}
		}

		/// <summary>
		/// Get Categories from web and insert it in the database.
		/// </summary>
		private async Task InitCategories()
		{
			try
			{
				await _elServiceCategory.InitCategories();
			}
			catch (Exception ex)
			{
				Logger.ServiceLog.Error(ex);
			}
		}

		private async Task InitHiddenCategories()
		{
			await _elServiceCategory.InitHiddenCategories();
		}

		protected override void ExecuteTryAgainCommand()
		{
			this.InitStories();
		}

		private void OnCurrentCategoryChanged()
		{
			_elServiceStory.ResetPageNumber();

			this.InitStories(true);
		}

		public override void OnNavigatedTo(INavigationParameters parameters)
		{
			if (AppSettings.DataSourceChanged)
			{
				_elServiceCategory.SetCurrentCategory(null);
			}
			else if (_elServiceCategory.HasCategorySelectionChanged(CurrentCategory))
			{
				CurrentCategory = _elServiceCategory.GetCurrentCategory();

				this.InitStories(true);
			}
		}
	}
}
