using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XStory.DTO;

namespace XStory.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private List<Story> _stories;
        private BL.Web.Contracts.IServiceStory _serviceStory;

        private StackLayout _storiesContainer;

        public DelegateCommand<string> StoriesItemTappedCommand { get; set; }
        public DelegateCommand StoriesItemAppearingCommand { get; set; }

        public DelegateCommand SettingsCommand { get; set; }

        public List<Story> Stories
        {
            get { return _stories; }
            set { SetProperty(ref _stories, value); }
        }

        public StackLayout StoriesContainer
        {
            get { return _storiesContainer; }
            set { SetProperty(ref _storiesContainer, value); }
        }

        public MainPageViewModel(INavigationService navigationService, BL.Web.Contracts.IServiceStory serviceStory)
            : base(navigationService)
        {
            Title = "Main Page";

            StoriesContainer = new StackLayout();

            AppearingCommand = new DelegateCommand(ExecuteAppearingCommand);
            SettingsCommand = new DelegateCommand(ExecuteSettingsCommand);
            StoriesItemTappedCommand = new DelegateCommand<string>((url) => ExecuteStoriesItemTappedCommand(url));
            StoriesItemAppearingCommand = new DelegateCommand(ExecuteStoriesItemAppearingCommand);

            _serviceStory = serviceStory;
        }

        private void MakeStoriesContainer()
        {
            // Clears the Grid
            StoriesContainer.Children.Clear();
            //StoriesContainer.RowDefinitions.Add(new RowDefinition());
            StoriesContainer.Children.Add(new Label() { Text = "lol" });

            //// Create a StoryInfoView (of type ContentView) and add it to the grid
            //for (int i = 0; i < Stories.Count; i++)
            //{
            //    Views.ContentViews.StoryView storyView = new Views.ContentViews.StoryView();
            //    storyView.BindingContext = Stories[i];

            //    StoriesContainer.Children.Add(storyView, 0, i + 1);
            //}
        }

        private void ExecuteStoriesItemAppearingCommand()
        {
            string s = "coucou";
        }

        private async void ExecuteSettingsCommand()
        {
            await NavigationService.NavigateAsync("SettingsPage");
        }

        private async void ExecuteStoriesItemTappedCommand(string url)
        {
            var navigationParams = new NavigationParameters()
            {
                { "storyUrl", url }
            };

            await NavigationService.NavigateAsync("StoryPage", navigationParams);
        }

        protected override async void ExecuteAppearingCommand()
        {
            Stories = await _serviceStory.GetStoriesMainPage(0, "");

            MakeStoriesContainer();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }
    }
}
