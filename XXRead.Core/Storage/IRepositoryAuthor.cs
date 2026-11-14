using XXRead.Core.Models;

namespace XXRead.Core.Storage
{
	/// <summary>
	/// Authors stored locally. Not used yet : groundwork to save a story with its author, or all the stories of an author.
	/// </summary>
	public interface IRepositoryAuthor
	{
		Task<List<Author>> GetAuthors();
		Task<Author> GetAuthor(string id);
		Task<int> InsertAuthor(Author author);
		Task<int> DeleteAuthor(Author author);
	}
}
