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

			foreach (Entities.Story storySQL in dbStories)
			{
				stories.Add(StoryMapper.ToStoryDTO(storySQL, await this.FindAuthor(storySQL.AuthorUrl)));
			}

			return stories;
		}

		public async Task<Story?> GetStory(string url)
		{
			Entities.Story? dbStory = await _database.GetInstance().FindAsync<Entities.Story>(url);
			if (dbStory == null) return null;

			return StoryMapper.ToStoryDTO(dbStory, await this.FindAuthor(dbStory.AuthorUrl));
		}

		private async Task<Entities.Author?> FindAuthor(string? authorUrl)
		{
			if (string.IsNullOrWhiteSpace(authorUrl)) return null;

			return await _database.GetInstance().FindAsync<Entities.Author>(authorUrl);
		}

		/// <summary>
		/// Saves the story and its author (when it has one) in a transaction.
		/// </summary>
		public async Task<int> InsertStory(Story story)
		{
			int affectedRows = 0;

			await _database.GetInstance().RunInTransactionAsync(connection =>
			{
				affectedRows = connection.InsertOrReplace(StoryMapper.ToStorySQLite(story));

				if (string.IsNullOrWhiteSpace(story.Author?.Url)) return;
				connection.InsertOrReplace(AuthorMapper.ToAuthorSQLite(story.Author));
			});

			return affectedRows;
		}

		public async Task<int> DeleteStory(Story story)
		{
			Entities.Story storySQL = StoryMapper.ToStorySQLite(story);
			return await _database.GetInstance().DeleteAsync(storySQL);
		}
	}
}
