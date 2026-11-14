using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class UnitTestsLayerHDS
    {
        [TestMethod]
        public void GetCategoriesTest_OK()
        {

            ISource _source = SourcesOnline.Get(DataSources.HDS);
            Task<List<Category>> task = _source.GetCategories();
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoriesMainPageTest_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            int page = 0;

            Task<List<Story>> task = _source.GetStoriesPage(0);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_01_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"l-artiste-antillais-44806";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_01_FIELDS_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"l-artiste-antillais-44806";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);

            string title = result.Title;
            Assert.IsFalse(string.IsNullOrWhiteSpace(title));

        }

        [TestMethod]
        public void GetStoryTest_02_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"collection-lesbiennes-gladys-(4-6)-28222";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_03_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"jours-de-soins-44823";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_04_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"le-defi-d-une-petite-cochonne--28373";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_05_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"marion-au-chateau-%28devenue-esclave-aphrodite-mde%29-%28suite-de-l-episode-precedent-n-1110%29-28067";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_06_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"on-s-est-fait-le-pere-noel--28291";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_07_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"le-baiser-de-tarentula-20h-28338";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetStoryTest_08_OK()
        {
            ISource _source = SourcesOnline.Get(DataSources.HDS);

            string url = @"envie-cachee-974-44706";

            Task<Story> task = _source.GetStory(url);
            var result = task.Result;

            Assert.IsNotNull(result);
        }
    }
}
