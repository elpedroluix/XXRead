using XXRead.Core.Models;
using XXRead.Core.Storage;
using XXRead.Storage.Mapping;

namespace XXRead.Storage
{
	internal class RepositoryStory : Repository, IRepositoryStory
	{
		public RepositoryStory(XXReadDatabase database) : base(database)
		{
		}

		public async Task<List<Story>> GetStories()
		{
			List<Story> stories = new List<Story>();

			List<Entities.Story> dbStories = await _database.GetInstance().QueryAsync<Entities.Story>("select * from Story");

			dbStories.ForEach(storySQL =>
			{
				stories.Add(StoryMapper.ToStoryDTO(storySQL));
			});

			return stories;
		}

		public async Task<Story> GetStory(string url)
		{
			Entities.Story dbStory = await _database.GetInstance().GetAsync<Entities.Story>(url);

			if (dbStory == null)
			{
				throw new Exception("Could not find Story in database.");

			}
			return StoryMapper.ToStoryDTO(dbStory);
		}

		public async Task<int> InsertStory(Story story)
		{
			Entities.Story storySQL = StoryMapper.ToStorySQLite(story);
			return await _database.GetInstance().InsertOrReplaceAsync(storySQL);
		}

		/// <summary>
		/// Inserts in a transaction : 
		/// 1. Story
		/// 2. AuthorStory
		/// 3. Author
		/// --- Rollback transaction if error.
		/// </summary>
		public async Task<int> InsertStoryWithAuthorTransac(Story story)
		{
			int insertResult = await _database.GetInstance().RunInTransactionAsync((conn) =>
			{
				conn.Insert(StoryMapper.ToStorySQLite(story));
				conn.Insert(new Entities.AuthorStory { AuthorId = story.Author.Id, StoryId = story.Url });
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

		public async Task<int> DeleteStory(Story story)
		{
			Entities.Story storySQL = StoryMapper.ToStorySQLite(story);
			return await _database.GetInstance().DeleteAsync(storySQL);
		}
	}
}
