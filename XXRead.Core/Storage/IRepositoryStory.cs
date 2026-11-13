using XStory.DTO;

namespace XXRead.Core.Storage
{
	/// <summary>
	/// Favorite stories, stored locally.
	/// </summary>
	public interface IRepositoryStory
	{
		Task<List<Story>> GetStories();
		Task<Story> GetStory(string url);
		Task<int> InsertStory(Story story);
		Task<int> DeleteStory(Story story);
	}
}
