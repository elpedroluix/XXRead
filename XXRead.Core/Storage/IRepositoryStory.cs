using XXRead.Core.Models;

namespace XXRead.Core.Storage
{
	/// <summary>
	/// Saved stories (« Histoires enregistrées »), stored locally with their author.
	/// </summary>
	public interface IRepositoryStory
	{
		Task<List<Story>> GetStories();
		Task<Story> GetStory(string url);

		/// <summary>
		/// Saves the story and its author (when it has one) in a transaction.
		/// </summary>
		Task<int> InsertStory(Story story);

		/// <summary>
		/// Inserts in a transaction : 
		/// 1. Story
		/// 2. AuthorStory
		/// 3. Author
		/// --- Rollback transaction if error.
		/// Not used yet by the app.
		/// </summary>
		/// <returns>1 on success, -1 on error.</returns>
		Task<int> InsertStoryWithAuthorTransac(Story story);

		Task<int> DeleteStory(Story story);
	}
}
