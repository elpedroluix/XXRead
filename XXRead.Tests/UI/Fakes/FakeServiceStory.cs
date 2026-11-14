using XXRead.Core.Models;
using XXRead.Core.Services;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// Only the members used by the MainPage and the StoryPage are implemented.
	/// </summary>
	public class FakeServiceStory : IServiceStory
	{
		/// <summary>
		/// Result of the next InitStories calls (null = error, as ServiceStory does).
		/// </summary>
		public Func<Task<List<Story>>> InitStoriesResult { get; set; } = () => Task.FromResult(new List<Story>());

		/// <summary>
		/// Result of the next InitStory calls (null = error). By default : the current story.
		/// </summary>
		public Func<Task<Story>> InitStoryResult { get; set; }

		public int InitStoriesCount { get; private set; }

		public int InitStoryCount { get; private set; }

		public Story CurrentStory { get; private set; }

		public List<Story> AlreadyLoadedStories { get; } = new List<Story>();

		public List<Story> SavedStories { get; } = new List<Story>();

		public Task<List<Story>> InitStories()
		{
			InitStoriesCount++;

			return InitStoriesResult();
		}

		public Task<Story> InitStory()
		{
			InitStoryCount++;

			return InitStoryResult == null ? Task.FromResult(CurrentStory) : InitStoryResult();
		}

		public List<Story> DistinctStories(List<Story> stories) => stories;

		public void SetCurrentStory(Story story) => CurrentStory = story;

		public Story GetCurrentStory() => CurrentStory;

		public void AddAlreadyLoadedStory(Story story) => AlreadyLoadedStories.Add(story);

		public Story GetAlreadyLoadedStory(Story story) => AlreadyLoadedStories.FirstOrDefault(s => s.Url == story.Url);

		public Task<Story> GetStorySQLite(Story story) => Task.FromResult(SavedStories.FirstOrDefault(s => s.Url == story.Url));

		public Task<int> InsertStorySQLite(Story story)
		{
			SavedStories.Add(story);

			return Task.FromResult(1);
		}

		public Task<List<Story>> LoadMoreStories() => throw new NotImplementedException();
		public Task<List<Story>> RefreshStories(Story firstStory) => throw new NotImplementedException();
		public void SetPageNumber(int value) => throw new NotImplementedException();
		public void IncrementPageNumber() => throw new NotImplementedException();
		public void ResetPageNumber() => throw new NotImplementedException();
		public Task<List<Story>> GetStoriesSQLite() => throw new NotImplementedException();
		public Task<int> DeleteStorySQLite(Story story) => throw new NotImplementedException();
	}
}
