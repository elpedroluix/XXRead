using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.Demo.Contracts;
using XStory.DTO;

namespace XStory.BL.Web.Demo
{
	public class ServiceAuthor : IServiceAuthor
	{
		public Task<Author> GetAuthorPage(Author author)
		{
			return Task.FromResult(author);
		}
	}
}
