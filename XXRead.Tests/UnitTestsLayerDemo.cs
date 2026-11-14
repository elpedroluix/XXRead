using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;
using XXRead.Core.Sources;
using XXRead.Web.Demo;

namespace XXRead.Tests
{
    [TestClass]
    public class UnitTestsLayerDemo
    {
        [TestMethod]
        public void GetListStoriesDemoTest_OK()
        {
            ISource _source = new SourceDemo();

            Task<List<Story>> task = _source.GetStoriesPage(0);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryDemoTest_OK()
        {
            ISource _source = new SourceDemo();

            Task<Story> task = _source.GetStory("");
            var result = task.Result;

            Assert.IsNotNull(result);
        }
    }
}
