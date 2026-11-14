using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DTO;

namespace XXRead.Core.Services
{
	public interface IServiceStory
	{
		Task<List<Story>> InitStories();
		Task<List<Story>> LoadMoreStories();
		Task<List<Story>> RefreshStories(Story firstStory);
		List<Story> DistinctStories(List<Story> stories);

		Task<Story> InitStory();
		Story GetCurrentStory();
		void SetCurrentStory(Story story);
		/// <summary>
		/// Add story in cache, if not already present
		/// </summary>
		/// <param name="story"></param>
		void AddAlreadyLoadedStory(Story story);
		Story GetAlreadyLoadedStory(Story story);

		void SetPageNumber(int value);
		void IncrementPageNumber();
		void ResetPageNumber();

		/* SQLite */

		Task<List<Story>> GetStoriesSQLite();
		Task<Story> GetStorySQLite(Story story);
		Task<int> InsertStorySQLite(Story story);
		Task<int> DeleteStorySQLite(Story story);
	}
}