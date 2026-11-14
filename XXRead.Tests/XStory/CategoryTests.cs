using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XXRead.Core.Models;
using XXRead.Core.Sources;

namespace XXRead.Tests.XStory
{
	[TestClass]
	public class CategoryTests
	{
		[TestMethod]
		public void GetCategoriesTest_OK()
		{

			ISource _source = SourcesOnline.Get(DataSources.XStory);
			Task<List<Category>> task = _source.GetCategories();
			var result = task.Result;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void GetStoriesByCategoryTest_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);
			int page = 0;

			string categoryUrl = "histoires-erotiques,gay,3.html";

			Task<List<Story>> task = _source.GetStoriesPage(page, categoryUrl);
			var result = task.Result;

			Assert.IsNotNull(result);
		}
	}
}
