using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.HDS;
using XStory.BL.Web.HDS.Contracts;
using XStory.DTO;

namespace XStory.XUnitTests
{
    [TestClass]
    public class UnitTestsLayerHDS
    {

        [TestMethod]
        public void GetStoriesMainPageTest_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            int page = 0;

            Task<List<Story>> task = _serviceStory.GetStoriesMainPage(page);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_01_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"l-artiste-antillais-44806";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_01_FIELDS_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"l-artiste-antillais-44806";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);

            string title = result.Title;
            Assert.IsFalse(string.IsNullOrWhiteSpace(title));

        }

        [TestMethod]
        public void GetStoryTest_02_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"collection-lesbiennes-gladys-(4-6)-28222";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_03_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"jours-de-soins-44823";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_04_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"le-defi-d-une-petite-cochonne--28373";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_05_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"marion-au-chateau-%28devenue-esclave-aphrodite-mde%29-%28suite-de-l-episode-precedent-n-1110%29-28067";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_06_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"on-s-est-fait-le-pere-noel--28291";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_07_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"le-baiser-de-tarentula-20h-28338";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_08_OK()
        {
            IServiceStory _serviceStory = new ServiceStory();

            string url = @"envie-cachee-974-44706";

            Task<Story> task = _serviceStory.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }
    }
}
