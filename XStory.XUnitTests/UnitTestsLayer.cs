using XStory.BL.Web;
using XStory.BL.Web.Contracts;
using XStory.DTO;

namespace XStory.XUnitTests
{
    [TestClass]
    public class UnitTestsLayer
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

            Task<List<Story>> task = _serviceStories.GetStoriesByCategory(page, "");
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoriesMainPageTest_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();
            
            int page = 0;

            Task<List<Story>> task = _serviceStory.GetStoriesMainPage(page, "");
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStory_OK()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("lire-histoire,femme-est-une-chienne,53291.html");
            var result = task.Result;

            Assert.IsNotNull(result);
        }
    }
}