using XXRead.Core.Models;
using XXRead.Core.Services;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// Only the members used by the MainPage are implemented.
	/// </summary>
	public class FakeServiceStory : IServiceStory
	{
		/// <summary>
		/// Result of the next InitStories calls (null = error, as ServiceStory does).
		/// </summary>
		public Func<Task<List<Story>>> InitStoriesResult { get; set; } = () => Task.FromResult(new List<Story>());

		public int InitStoriesCount { get; private set; }

		public Story CurrentStory { get; private set; }

		public Task<List<Story>> InitStories()
		{
			InitStoriesCount++;

			return InitStoriesResult();
		}

		public List<Story> DistinctStories(List<Story> stories) => stories;

		public void SetCurrentStory(Story story) => CurrentStory = story;

		public Story GetCurrentStory() => CurrentStory;

		public Task<List<Story>> LoadMoreStories() => throw new NotImplementedException();
		public Task<List<Story>> RefreshStories(Story firstStory) => throw new NotImplementedException();
		public Task<Story> InitStory() => throw new NotImplementedException();
		public void AddAlreadyLoadedStory(Story story) => throw new NotImplementedException();
		public Story GetAlreadyLoadedStory(Story story) => throw new NotImplementedException();
		public void SetPageNumber(int value) => throw new NotImplementedException();
		public void IncrementPageNumber() => throw new NotImplementedException();
		public void ResetPageNumber() => throw new NotImplementedException();
		public Task<List<Story>> GetStoriesSQLite() => throw new NotImplementedException();
		public Task<Story> GetStorySQLite(Story story) => throw new NotImplementedException();
		public Task<int> InsertStorySQLite(Story story) => throw new NotImplementedException();
		public Task<int> DeleteStorySQLite(Story story) => throw new NotImplementedException();
	}
}
