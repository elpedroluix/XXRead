using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using XStory.BL.Web.HDS;
using XStory.BL.Web.HDS.Contracts;
using XStory.DAL.SQLite;
using XStory.DTO;

namespace XStory.XUnitTests.HDS
{
	[TestClass]
	public class HDSBackupTests
	{
		[TestMethod]
		public void GetAllStoriesAndInsertInDB_OK()
		{
			BL.Web.DSLocator.Contracts.IServiceStory _serviceStory = new BL.Web.DSLocator.ServiceStory(new BL.Web.XStory.ServiceStory(), new BL.Web.HDS.ServiceStory(), null);
			BL.SQLite.Contracts.IServiceStory _serviceStorySQLite = new BL.SQLite.ServiceStory(
				new DAL.SQLite.RepositoryStory(new XXReadDatabase()));

			bool isFinished = false;
			do
			{
				var stories = await _serviceStory.GetStoriesPage("HDS", _pageNumber);
				if (stories == null || stories.Count == 0)
				{
					isFinished = true;
					break;
				}
				InsertedStories += await this.InsertStories(stories);

				_pageNumber++;
			} while (!isFinished);
		}

		private async Task<long> InsertStories(List<DTO.Story> stories)
		{
			long insertedStories = 0;
			foreach (XStory.DTO.Story story in stories)
			{
				insertedStories = await _serviceStorySQLite.InsertStoryWithAuthorTransac(story) ? insertedStories++ : insertedStories;
			}
			return insertedStories;
		}
	}
}
