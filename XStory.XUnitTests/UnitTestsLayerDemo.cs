using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.XUnitTests
{
    [TestClass]
    public class UnitTestsLayerDemo
    {
        [TestMethod]
        public void GetListStoriesDemoTest_OK()
        {
            BL.Web.Demo.Contracts.IServiceStory _serviceStory = new BL.Web.Demo.ServiceStory();

            Task<List<Story>> task = _serviceStory.GetStoriesPage();
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryDemoTest_OK()
        {
            BL.Web.Demo.Contracts.IServiceStory _serviceStory = new BL.Web.Demo.ServiceStory();

            Task<Story> task = _serviceStory.GetStory("");
            var result = task.Result;

            Assert.IsNotNull(result);
        }
    }
}
