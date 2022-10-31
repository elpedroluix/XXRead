using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.SQLite.Contracts;
using XStory.DAL.SQLite;
using XStory.DAL.SQLite.Contracts;
using XStory.DTO;

namespace XStory.BL.SQLite
{
    public class ServiceStory : IServiceStory
    {
        private IRepositoryStory _repositoryStory;
        public ServiceStory()
        {
            _repositoryStory = new RepositoryStory();
        }

        public Task<List<Story>> GetStories()
        {
            throw new NotImplementedException();
        }

        public Task<Story> GetStory(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertStory(Story story)
        {
            return await _repositoryStory.InsertStory(story);
        }
    }
}
