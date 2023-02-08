using XStory.BL.Web;
using XStory.BL.Web.Contracts;
using XStory.DTO;

namespace XStory.XUnitTests
{
    [TestClass]
    public class UnitTestsLayerXStory
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

        [TestMethod]
        public void GetStory_OK_01_TypeAucun()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("/lire-histoire,garcon-timide-fille-epanouie,56348.html");
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStory_OK_02_TypeFantasme()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("/lire-histoire,the-walking-dicks,56341.html");
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStory_OK_03_TypeHistoireVraie()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("lire-histoire,ami-gay-copine,56384.html");
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStory_OK_04_ViewsNumber()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("/lire-histoire,des-vacances-salee,56429.html");
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        

        [TestMethod]
        public void GetStoryWithChapters_OK()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("lire-histoire,service-vraiment-tres-particulier,53361.html");
            var result = task.Result;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ChaptersList);
        }

        [TestMethod]
        public void GetStoryWithNotAllChaptersNamed_OK()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("lire-histoire,tante-soeur,54397.html");
            var result = task.Result;

            Assert.IsNotNull(result);
            foreach (var chapter in result.ChaptersList)
            {
                Assert.AreNotEqual("", chapter.Title);
            }
        }


        [TestMethod]
        public void GetStoryMostViewed_OK()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("/lire-histoire,sauvetage-une-maman,49583.html");
            var result = task.Result;

            Assert.IsNotNull(result);
        }
    }
}