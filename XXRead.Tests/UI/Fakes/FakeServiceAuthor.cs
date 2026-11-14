using XXRead.Core.Models;
using XXRead.Core.Services;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// Only the members used by the StoryPage and the AuthorPage are implemented.
	/// </summary>
	public class FakeServiceAuthor : IServiceAuthor
	{
		/// <summary>
		/// Result of the next InitAuthor calls (null = error, as ServiceAuthor does). By default : the current author.
		/// </summary>
		public Func<Task<Author>> InitAuthorResult { get; set; }

		/// <summary>
		/// Next page of the author's stories. By default : the author as is (as ServiceAuthor does on error).
		/// </summary>
		public Func<Author, Task<Author>> NextPageResult { get; set; } = author => Task.FromResult(author);

		public int InitAuthorCount { get; private set; }

		public int NextPageCount { get; private set; }

		public Author CurrentAuthor { get; private set; }

		public List<Author> AlreadyLoadedAuthors { get; } = new List<Author>();

		public void SetCurrentAuthor(Author author) => CurrentAuthor = author;

		public Author GetCurrentAuthor() => CurrentAuthor;

		public Task<Author> InitAuthor()
		{
			InitAuthorCount++;

			return InitAuthorResult == null ? Task.FromResult(CurrentAuthor) : InitAuthorResult();
		}

		public void AddAlreadyLoadedAuthor(Author author) => AlreadyLoadedAuthors.Add(author);

		public Author GetAlreadyLoadedAuthor(Author author) => AlreadyLoadedAuthors.FirstOrDefault(a => a.Url == author.Url);

		public Task<Author> GetAuthorStoriesNextPage(Author author)
		{
			NextPageCount++;

			return NextPageResult(author);
		}

		public void UpdateAlreadyLoadedAuthor(Author author) => throw new NotImplementedException();
	}
}
