using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;
using XStory.Logger;
using XXRead.Core.Session;
using XXRead.Core.Sources;

namespace XXRead.Core.Services
{
	public class ServiceAuthor : IServiceAuthor
	{
		private IServiceSource _serviceSource;
		private IServiceSession _serviceSession;

		public ServiceAuthor(IServiceSource serviceSource,
			IServiceSession serviceSession)
		{
			_serviceSource = serviceSource;
			_serviceSession = serviceSession;
		}

		/// <summary>
		/// On error, returns the author as is (same as the former HDS / XStory services) :
		/// AuthorPageViewModel does not handle null when loading the next page.
		/// </summary>
		private async Task<Author> GetAuthorPage(Author author, int pageNumber = 1)
		{
			try
			{
				return await _serviceSource.Current.GetAuthorPage(author, pageNumber);
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return author;
			}
		}

		public void AddAlreadyLoadedAuthor(Author author)
		{
			if (_serviceSession.ListAlreadyLoadedAuthors.FirstOrDefault(ala => ala.Url == author?.Url) == null)
			{
				_serviceSession.ListAlreadyLoadedAuthors.Add(author);
			}
		}

		public void UpdateAlreadyLoadedAuthor(Author author)
		{
			if (author == null)
			{
				return;
			}

			int index = _serviceSession.ListAlreadyLoadedAuthors.FindIndex(ala => ala.Url == author.Url);
			if (index != -1)
			{
				_serviceSession.ListAlreadyLoadedAuthors[index] = author;
			}
		}

		public Author GetAlreadyLoadedAuthor(Author author)
		{
			return _serviceSession.ListAlreadyLoadedAuthors.FirstOrDefault(ala => ala.Url.Contains(author.Url));
		}

		public async Task<Author> GetAuthorStoriesNextPage(Author author)
		{
			// Page number kept by author : authors are cached with their already loaded pages
			int pageNumber = _serviceSession.AuthorPageNumbers.GetValueOrDefault(author.Url, 1) + 1;
			try
			{
				Author authorNextPage = await _serviceSource.Current.GetAuthorPage(author, pageNumber);

				// Only on success, otherwise the next try would skip a page
				_serviceSession.AuthorPageNumbers[author.Url] = pageNumber;
				this.UpdateAlreadyLoadedAuthor(authorNextPage);

				return authorNextPage;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return author;
			}
		}

		public Author GetCurrentAuthor()
		{
			return _serviceSession.CurrentAuthor;
		}

		public async Task<Author> InitAuthor()
		{
			try
			{
				Author author = await this.GetAuthorPage(_serviceSession.CurrentAuthor);
				this.UpdateAlreadyLoadedAuthor(author);

				if (author?.Url != null)
				{
					_serviceSession.AuthorPageNumbers[author.Url] = 1;
				}

				return author;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return null;
			}
		}

		public void SetCurrentAuthor(Author author)
		{
			_serviceSession.CurrentAuthor = author;
		}
	}
}
