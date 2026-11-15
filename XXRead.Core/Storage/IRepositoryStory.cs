using XXRead.Core.Models;

namespace XXRead.Core.Storage
{
	/// <summary>
	/// Saved stories (« Histoires enregistrées »), stored locally with their author.
	/// </summary>
	public interface IRepositoryStory
	{
		Task<List<Story>> GetStories();
		/// <returns>null when the story is not saved (not an error).</returns>
		Task<Story?> GetStory(string url);

		/// <summary>
		/// Saves the story and its author (when it has one) in a transaction.
		/// </summary>
		Task<int> InsertStory(Story story);

		Task<int> DeleteStory(Story story);
	}
}
