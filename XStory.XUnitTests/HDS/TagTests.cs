using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.HDS;
using XStory.BL.Web.HDS.Contracts;
using XStory.DTO;

namespace XStory.XUnitTests.HDS
{
	[TestClass]
	public class TagTests
	{
		[TestMethod]
		public void GetAllTags_OK()
		{
			IServiceTag _serviceTag = new ServiceTag(new DAL.Web.HDS.RepositoryWebHDS(), new BL.Web.HDS.ServiceStory());

			Task<List<Tag>> task = _serviceTag.GetTags();
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetTagsStories_OK()
		{
			//IServiceAuthor _serviceAuthor = new ServiceAuthor(new DAL.Web.HDS.RepositoryWebHDS(), new BL.Web.HDS.ServiceStory());

			//Author author = new Author()
			//{
			//	Name = "Musty",
			//	Url = "https://www.histoires-de-sexe.net/fiche.php?auteur=Musty"
			//};

			//Task<Author> task = _serviceAuthor.GetAuthorPage(author);
			//var result = task.Result;

			//Assert.IsNotNull(result.Id);
		}
	}
}
