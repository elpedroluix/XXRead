using XStory.Models;

namespace XStory.XTestsProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetCategoriesTest_OK(){
            XStory.Helpers.DataAccess.Service _service = new Helpers.DataAccess.Service();
            Task<List<Category>> task = _service.GetCategories();
            var result = task.Result;

            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void GetStoriesByCategoryTest_OK(){
            XStory.Helpers.DataAccess.Service _service = new Helpers.DataAccess.Service();
            int page = 0;
            
            Task<List<Story>> task = _service.GetStoriesByCategory(page);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoriesMainPageTest_OK()
        {
            XStory.Helpers.DataAccess.Service _service = new Helpers.DataAccess.Service();
            int page = 0;

            Task<List<Story>> task = _service.GetStoriesMainPage(page);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStory_OK()
        {
            XStory.Helpers.DataAccess.Service _service = new Helpers.DataAccess.Service();

            Task<Story> task = _service.GetStory("lire-histoire,femme-est-une-chienne,53291.html");
            var result = task.Result;

            Assert.IsNotNull(result);
        }
    }
}