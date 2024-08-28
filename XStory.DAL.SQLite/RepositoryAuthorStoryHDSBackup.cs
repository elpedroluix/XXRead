using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DAL.SQLite.Contracts;
using XStory.DAL.SQLite.Mapping;
using XStory.DAL.SQLiteObjects;
using XStory.Logger;

namespace XStory.DAL.SQLite
{
	public class RepositoryAuthorStoryHDSBackup : RepositoryHDSBackup, IRepositoryAuthorStoryHDSBackup
	{
		public RepositoryAuthorStoryHDSBackup(IXXReadDatabaseHDSBackup database) : base(database)
		{
			_database = database;
		}

		public async Task<int> InsertAuthorStory(DTO.Story story)
		{
			try
			{
				var x = await _database.GetInstance().InsertAsync(new SQLiteObjects.AuthorStory { AuthorId = story.Author.Id, StoryId = story.Url });
				return x;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return -1;
			}
		}

		public async Task<int> InsertAuthorStoryTransac(DTO.Story story)
		{
			int insertResult = await _database.GetInstance().RunInTransactionAsync((conn) =>
			{
				conn.InsertOrReplace(StoryMapper.ToStorySQLite(story));
				conn.InsertOrReplace(new SQLiteObjects.AuthorStory { AuthorId = story.Author.Id, StoryId = story.Url });
				conn.InsertOrReplace(AuthorMapper.ToAuthorSQLite(story.Author));

				conn.Commit();
			}).ContinueWith<int>((insertTask) =>
				{
					if (insertTask.IsFaulted)
					{
						return -1;
					}
					else if (insertTask.IsCompleted)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				});

			return insertResult;
		}
	}
}
