using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core.Sources;

namespace XXRead.Tests
{
    [TestClass]
    public class UnitTestsLayerCommon
    {
        [TestMethod]
        public void GetListStoriesXStoryTest_OK()
        {
            IServiceSource _serviceSource = SourcesOnline.ServiceSource;

            Task<List<Story>> task = _serviceSource.Get(DataSources.XStory).GetStoriesPage(0);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetListStoriesHDSTest_OK()
        {
            IServiceSource _serviceSource = SourcesOnline.ServiceSource;

            Task<List<Story>> task = _serviceSource.Get(DataSources.HDS).GetStoriesPage(2);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetListStoriesDemoTest_OK()
        {
            IServiceSource _serviceSource = SourcesOnline.ServiceSource;

            Task<List<Story>> task = _serviceSource.Get(DataSources.Demo).GetStoriesPage(0);
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
    }
}
