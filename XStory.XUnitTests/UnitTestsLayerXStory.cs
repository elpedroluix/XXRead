using XStory.BL.Web.XStory;
using XStory.BL.Web.XStory.Contracts;
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

            string categoryUrl = "histoires-erotiques,gay,3.html";

            Task<List<Story>> task = _serviceStories.GetStoriesPage(page, categoryUrl);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoriesMainPageTest_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            int page = 0;

            Task<List<Story>> task = _serviceStory.GetStoriesPage(page);
            var result = task.Result;

            //string json = System.Text.Json.JsonSerializer.Serialize(result);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetFilteredStoriesMainPageTest_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            int page = 0;

            List<string> categs = new List<string>() { "histoires-erotiques,zoophilie,10.html", "histoires-erotiques,inceste,12.html" };

            Task<List<Story>> task = _serviceStory.GetStoriesPage(page);

            var result = _serviceStory.FilterStories(task.Result,categs);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetFilteredStoriesMainPageTest_OK1()
        {
            IServiceStory _serviceStory = new ServiceStory();

            int page = 0;

            List<string> categs = new List<string>() { "histoires-erotiques,gay,3.html", "histoires-erotiques,zoophilie,10.html", "histoires-erotiques,inceste,12.html", "histoires-erotiques,erotique,14.html" };

            Task<List<Story>> task = _serviceStory.GetStoriesPage(page);

            var result = _serviceStory.FilterStories(task.Result, categs);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetFilteredStoriesMainPageTest_OK_EmptyFilters()
        {
            IServiceStory _serviceStory = new ServiceStory();

            int page = 0;

            List<string> categs = new List<string>();

            Task<List<Story>> task = _serviceStory.GetStoriesPage(page);

            var result = _serviceStory.FilterStories(task.Result, categs);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetFilteredStoriesMainPageTest_OK_NullFilters()
        {
            IServiceStory _serviceStory = new ServiceStory();

            int page = 0;

            List<string> categs = null;

            Task<List<Story>> task = _serviceStory.GetStoriesPage(page);

            var result = _serviceStory.FilterStories(task.Result, categs);

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

            Task<Story> task = _serviceStories.GetStory("lire-histoire,fiona-journaliste-sportive,46905.html");
            var result = task.Result;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ChaptersList);
        }

        [TestMethod]
        public void GetStoryWithChaptersJSON_OK()
        {
            IServiceStory _serviceStories = new ServiceStory();

            Task<Story> task = _serviceStories.GetStory("lire-histoire,fiona-journaliste-sportive,46905.html");
            var result = task.Result;

            string json = System.Text.Json.JsonSerializer.Serialize(result);

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