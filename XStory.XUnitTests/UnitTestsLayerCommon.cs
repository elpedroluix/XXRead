using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.DSLocator.Contracts;
using XStory.DTO;

namespace XStory.XUnitTests
{
    [TestClass]
    public class UnitTestsLayerCommon
    {
        public const string DS_XSTORY = "XStory";
        public const string DS_HDS = "HDS";
        public const string DS_DEMO = "Demo";

        [TestMethod]
        public void GetListStoriesXStoryTest_OK()
        {
            BL.Web.DSLocator.Contracts.IServiceStory _serviceStory = InitMultiServiceStory();

            Task<List<Story>> task = _serviceStory.GetStoriesPage(DS_XSTORY);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetListStoriesXStoryTestWithFilters_OK()
        {
            BL.Web.DSLocator.Contracts.IServiceStory _serviceStory = InitMultiServiceStory();

            Task<List<Story>> task = _serviceStory.GetStoriesPage(DS_XSTORY);
            var result = task.Result;

            var result2 = _serviceStory.FilterStories(DS_XSTORY,result,new List<string> { "histoires-erotiques,zoophilie,10.html", "histoires-erotiques,inceste,12.html" });

            Assert.IsNotNull(result2);
        }

        [TestMethod]
        public void GetListStoriesXStoryTestWithFiltersWrongDS_OK()
        {
            BL.Web.DSLocator.Contracts.IServiceStory _serviceStory = InitMultiServiceStory();

            Task<List<Story>> task = _serviceStory.GetStoriesPage(DS_XSTORY);
            var result = task.Result;

            var result2 = _serviceStory.FilterStories(DS_HDS, result, new List<string> { "histoires-erotiques,zoophilie,10.html", "histoires-erotiques,inceste,12.html" });

            Assert.IsNull(result2);
        }

        [TestMethod]
        public void GetListStoriesHDSTest_OK()
        {
            BL.Web.DSLocator.Contracts.IServiceStory _serviceStory = InitMultiServiceStory();

            Task<List<Story>> task = _serviceStory.GetStoriesPage(DS_HDS, 2);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetListStoriesDemoTest_OK()
        {
            BL.Web.DSLocator.Contracts.IServiceStory _serviceStory = InitMultiServiceStory();

            Task<List<Story>> task = _serviceStory.GetStoriesPage(DS_DEMO);
            var result = task.Result;

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void GetStoryDemoTest_OK()
        {
            //BL.Web.Demo.Contracts.IServiceStory _serviceStory = new BL.Web.Demo.ServiceStory();

            //Task<Story> task = _serviceStory.GetStory("");
            //var result = task.Result;

            //Assert.IsNotNull(result);
        }

        private IServiceStory InitMultiServiceStory()
        {
            return new BL.Web.DSLocator.ServiceStory(
                new BL.Web.XStory.ServiceStory(),
                new BL.Web.HDS.ServiceStory(),
                new BL.Web.Demo.ServiceStory());
        }
    }
}
