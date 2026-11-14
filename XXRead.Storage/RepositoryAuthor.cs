using XStory.DTO;
using XXRead.Core.Storage;
using XXRead.Storage.Mapping;

namespace XXRead.Storage
{
	/// <summary>
	/// Not used yet : groundwork to save a story with its author, or all the stories of an author.
	/// </summary>
	internal class RepositoryAuthor : Repository, IRepositoryAuthor
	{
		public RepositoryAuthor(XXReadDatabase database) : base(database)
		{
		}

		public async Task<List<Author>> GetAuthors()
		{
			// TODO : not implemented
			throw new NotImplementedException();
		}

		public async Task<Author> GetAuthor(string id)
		{
			Entities.Author dbAuthor = await _database.GetInstance().GetAsync<Entities.Author>(id);

			if (dbAuthor == null)
			{
				throw new Exception($"Could not find specified Author (using id:{id}) in database.");

			}
			return AuthorMapper.ToAuthorDTO(dbAuthor);
		}

		public async Task<int> InsertAuthor(Author author)
		{
			Entities.Author authorSQL = AuthorMapper.ToAuthorSQLite(author);

			return await _database.GetInstance().InsertAsync(authorSQL);
		}

		public async Task<int> DeleteAuthor(Author author)
		{
			// TODO : deletes a string (author.Id), not an Entities.Author : throws, no table is mapped to string
			return await _database.GetInstance().DeleteAsync(author.Id);
		}
	}
}
