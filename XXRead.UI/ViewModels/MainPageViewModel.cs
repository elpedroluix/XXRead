using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using XXRead.Core.Models;
using XXRead.Core.Services;
using XXRead.UI.Constants;
using XXRead.UI.Dialogs;
using XXRead.UI.Platform;
using XXRead.UI.Services;

namespace XXRead.UI.ViewModels
{
	/// <summary>
	/// Copied from the XAML UI's MainPageViewModel. Scoped : the stories are kept when the user comes back to the page.
	/// </summary>
	public class MainPageViewModel : BaseViewModel
	{
		#region --- Fields ---
		private IServiceStory _serviceStory;
		private IServiceCategory _serviceCategory;
		private IServiceDialog _serviceDialog;
		private IServiceAppSettings _serviceAppSettings;

		private ObservableCollection<Story> _stories;
		public ObservableCollection<Story> Stories
		{
			get { return _stories; }
			set { SetProperty(ref _stories, value); }
		}

		private Category _currentCategory;
		/// <summary>
		/// Null = all categories.
		/// </summary>
		public Category CurrentCategory
		{
			get { return _currentCategory; }
			set { SetProperty(ref _currentCategory, value); }
		}

		/// <summary>
		/// Scroll position of the list when a story is opened, restored by the page when the user comes back.
		/// </summary>
		public double ScrollPosition { get; set; }
		#endregion

		#region --- Commands ---
		public RelayCommand CategoryTappedCommand { get; set; }
		public RelayCommand LoadMoreStoriesCommand { get; set; }
		public RelayCommand<Story> StoriesItemTappedCommand { get; set; }
		/// <summary>
		/// Async : the pull-to-refresh indicator stays until the refresh is done.
		/// </summary>
		public AsyncRelayCommand StoriesRefreshCommand { get; set; }
		#endregion

		#region --- Ctor ---
		public MainPageViewModel(IServiceNavigation serviceNavigation,
			IServiceDialog serviceDialog,
			IServiceAppSettings serviceAppSettings,
			IServiceStory serviceStory,
			IServiceCategory serviceCategory) : base(serviceNavigation)
		{
			_serviceStory = serviceStory;
			_serviceCategory = serviceCategory;
			_serviceDialog = serviceDialog;
			_serviceAppSettings = serviceAppSettings;

			Title = MainPageConstants.MAINPAGE_TITLE;
			ViewState = ViewStateEnum.Loading;

			CategoryTappedCommand = new RelayCommand(ExecuteCategoryTappedCommand);
			LoadMoreStoriesCommand = new RelayCommand(ExecuteLoadMoreStoriesCommand);
			StoriesItemTappedCommand = new RelayCommand<Story>(ExecuteStoriesItemTappedCommand);
			StoriesRefreshCommand = new AsyncRelayCommand(ExecuteStoriesRefreshCommand);
			TryAgainCommand = new RelayCommand(ExecuteTryAgainCommand);
		}
		#endregion

		/// <summary>
		/// Loads the stories the first time, and again when the data source has changed in the settings.
		/// </summary>
		public override async Task OnAppearing()
		{
			if (_serviceAppSettings.FirstRun)
			{
				// Not awaited : the stories are loaded behind the message
				_ = _serviceDialog.ShowMessage(MainPageConstants.MAINPAGE_FIRST_RUN_TITLE, MainPageConstants.MAINPAGE_FIRST_RUN_MESSAGE);

				_serviceAppSettings.FirstRun = false;
			}

			if (Stories != null && !_serviceAppSettings.DataSourceChanged)
			{
				return;
			}

			Stories = null;
			ScrollPosition = 0;
			CurrentCategory = _serviceCategory.GetCurrentCategory();

			await InitCategories();
			await InitStories();
		}

		/// <summary>
		/// Display Categories selection dialog
		/// </summary>
		private async void ExecuteCategoryTappedCommand()
		{
			Category currentCategory = _serviceCategory.GetCurrentCategory();

			CategorySelection selection = await _serviceDialog.ShowAsync<DialogSelectCategory, CategorySelection>();
			if (selection == null || selection.Category == currentCategory)
			{
				return;
			}

			CurrentCategory = selection.Category;
			_serviceCategory.SetCurrentCategory(selection.Category);

			// TODO : the page number is not reset, the new category starts at the page reached by "Charger plus"
			// TODO : InitStories returns null on error : ArgumentNullException in this async void
			Stories = new ObservableCollection<Story>(await _serviceStory.InitStories());
		}

		private async void ExecuteStoriesItemTappedCommand(Story story)
		{
			if (story == null)
			{
				return;
			}

			_serviceStory.SetCurrentStory(story);

			await ServiceNavigation.NavigateTo<Pages.StoryPage>();
		}

		/// <summary>
		/// When the PullToRefresh is triggered on StoriesList.
		/// If the 1st item of the two lists is different : refresh.
		/// Else : nothing
		/// </summary>
		private async Task ExecuteStoriesRefreshCommand()
		{
			try
			{
				if (Stories == null || Stories.Count == 0)
				{
					Stories = new ObservableCollection<Story>(await _serviceStory.InitStories());
					return;
				}

				List<Story> refreshList = await _serviceStory.RefreshStories(Stories.First());
				if (refreshList == null)
				{
					throw new Exception("Couldn't refresh stories");
				}

				if (refreshList.Count > 0)
				{
					Stories = new ObservableCollection<Story>(refreshList);
				}
			}
			catch (Exception ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
			}
		}

		private async void ExecuteLoadMoreStoriesCommand()
		{
			// TODO : LoadMoreStories returns null on error : NRE in this async void (fixed in step 8.8)
			List<Story> moreStories = await _serviceStory.LoadMoreStories();

			moreStories.ForEach(story => _stories.Add(story));

			Stories = new ObservableCollection<Story>(_serviceStory.DistinctStories(_stories.ToList()));
		}

		private async Task InitStories()
		{
			ViewState = ViewStateEnum.Loading;

			List<Story> stories = await _serviceStory.InitStories();
			_serviceAppSettings.DataSourceChanged = false;

			if (stories == null)
			{
				ViewState = ViewStateEnum.Error;
				return;
			}

			Stories = new ObservableCollection<Story>(stories);
			ViewState = ViewStateEnum.Display;
		}

		/// <summary>
		/// Get Categories from web and insert it in the database.
		/// </summary>
		private async Task InitCategories()
		{
			try
			{
				await _serviceCategory.InitCategories();
			}
			catch (Exception ex)
			{
				XXRead.Logger.ServiceLog.Error(ex);
			}
		}

		protected override async void ExecuteTryAgainCommand()
		{
			await InitStories();
		}
	}
}
