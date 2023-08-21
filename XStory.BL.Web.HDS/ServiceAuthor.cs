using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.HDS.Contracts;
using XStory.DTO;

namespace XStory.BL.Web.HDS
{
	public class ServiceAuthor : IServiceAuthor
	{
		public Task<Author> GetAuthorPage(Author author)
		{
			return Task.FromResult(author);
		}
	}
}
