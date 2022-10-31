using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DAL.SQLite.Contracts;
using XStory.DAL.SQLite.Mapping;
using XStory.DTO;

namespace XStory.DAL.SQLite
{
    public class RepositoryStory : Repository, IRepositoryStory
    {
        public async Task<List<Story>> GetStories()
        {
            List<DTO.Story> stories = new List<DTO.Story>();

            List<SQLiteObjects.Story> dbStories = await SQLConnection.QueryAsync<SQLiteObjects.Story>("select * from Story");

            dbStories.ForEach(storySQL =>
            {
                stories.Add(StoryMapper.ToStoryDTO(storySQL));
            });

            return stories;
        }

        public async Task<Story> GetStory(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertStory(Story story)
        {
            SQLiteObjects.Story storySQL = Mapping.StoryMapper.ToStorySQLite(story);

            return await SQLConnection.InsertAsync(storySQL);
        }
    }
}
