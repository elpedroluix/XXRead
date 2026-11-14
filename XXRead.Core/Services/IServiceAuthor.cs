using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;

namespace XXRead.Core.Services
{
	public interface IServiceAuthor
	{
		Task<Author> InitAuthor();
		Author GetCurrentAuthor();
		void SetCurrentAuthor(Author author);

		/// <summary>
		/// Add story in cache, if not already present
		/// </summary>
		/// <param name="author"></param>
		void AddAlreadyLoadedAuthor(Author author);
		void UpdateAlreadyLoadedAuthor(Author author);
		Author GetAlreadyLoadedAuthor(Author author);
		Task<Author> GetAuthorStoriesNextPage(Author author);
	}
}
