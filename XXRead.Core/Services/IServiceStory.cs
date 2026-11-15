using System.Collections.Generic;
using System.Threading.Tasks;
using XXRead.Core.Models;

namespace XXRead.Core.Services
{
	public interface IServiceStory
	{
		/// <returns>null on error.</returns>
		Task<List<Story>> InitStories();
		/// <returns>Empty list on error, the page number is kept so that the next call asks for the same page.</returns>
		Task<List<Story>> LoadMoreStories();
		/// <returns>Empty list when the first story has not changed, null on error.</returns>
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
		/// <returns>null when the story is not saved, or on error.</returns>
		Task<Story?> GetStorySQLite(Story story);
		Task<int> InsertStorySQLite(Story story);
		Task<int> DeleteStorySQLite(Story story);
	}
}