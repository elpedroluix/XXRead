using XXRead.Core.Models;

namespace XXRead.Storage.Mapping
{
	internal class AuthorMapper
	{
		public static Author ToAuthorDTO(Entities.Author author)
		{
			return new Author()
			{
				Id = author.Id,
				Name = author.Name,
				Url = author.Url
			};
		}

		public static Entities.Author ToAuthorSQLite(Author author)
		{
			return new Entities.Author()
			{
				Id = author.Id,
				Name = author.Name,
				Url = author.Url
			};
		}
	}
}
