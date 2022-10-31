using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XStory.XUnitTests
{
    [TestClass]
    public class UnitTestsSQLite
    {
        [TestMethod]
        public void GetDatabasePathTest_OK()
        {
            XStory.BL.SQLite.Contracts.IServiceStory _serviceStory = new BL.SQLite.ServiceStory();

            // Task<string> task = _serviceStory.GetDatabasePath();
        }

        [TestMethod]
        public void GetStoriesTest_OK()
        {

        }

        [TestMethod]
        public void InsertStoryTest_OK()
        {

        }

        [TestMethod]
        public void GetStoryTest_OK()
        {

        }
    }
}
