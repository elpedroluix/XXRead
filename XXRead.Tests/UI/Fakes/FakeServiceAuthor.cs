using XXRead.Core.Models;
using XXRead.Core.Services;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// Only the members used by the StoryPage are implemented.
	/// </summary>
	public class FakeServiceAuthor : IServiceAuthor
	{
		public Author CurrentAuthor { get; private set; }

		public void SetCurrentAuthor(Author author) => CurrentAuthor = author;

		public Author GetCurrentAuthor() => CurrentAuthor;

		public Task<Author> InitAuthor() => throw new NotImplementedException();
		public void AddAlreadyLoadedAuthor(Author author) => throw new NotImplementedException();
		public void UpdateAlreadyLoadedAuthor(Author author) => throw new NotImplementedException();
		public Author GetAlreadyLoadedAuthor(Author author) => throw new NotImplementedException();
		public Task<Author> GetAuthorStoriesNextPage(Author author) => throw new NotImplementedException();
	}
}
