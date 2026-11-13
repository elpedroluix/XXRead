using XStory.DTO;

namespace XXRead.Core.Storage
{
	public interface IRepositoryAuthor
	{
		Task<List<Author>> GetAuthors();
		Task<Author> GetAuthor(string id);
		Task<int> InsertAuthor(Author author);
		Task<int> DeleteAuthor(Author author);
	}
}
