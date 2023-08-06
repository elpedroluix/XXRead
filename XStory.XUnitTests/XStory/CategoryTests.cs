using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.XStory;
using XStory.BL.Web.XStory.Contracts;
using XStory.DTO;

namespace XStory.XUnitTests.XStory
{
	[TestClass]
	public class CategoryTests
	{
		[TestMethod]
		public void GetCategoriesTest_OK()
		{

			IServiceCategory _serviceCategory = new ServiceCategory();
			Task<List<Category>> task = _serviceCategory.GetCategories();
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStoriesByCategoryTest_OK()
		{
			IServiceStory _serviceStories = new ServiceStory();
			int page = 0;

			string categoryUrl = "histoires-erotiques,gay,3.html";

			Task<List<Story>> task = _serviceStories.GetStoriesPage(page, categoryUrl);
			var result = task.Result;

			Assert.IsNotNull(result);
		}
	}
}
