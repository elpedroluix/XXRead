using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Web.XStory.Contracts
{
	public interface IServiceAuthor
	{
		Task<DTO.Author> GetAuthorPage(Author author);
	}
}
