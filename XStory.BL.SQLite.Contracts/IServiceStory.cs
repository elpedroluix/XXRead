using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.BL.SQLite.Contracts
{
    public interface IServiceStory
    {
        Task<List<DTO.Story>> GetStories();
        Task<DTO.Story> GetStory(string url);
        Task<int> InsertStory(DTO.Story story);
    }
}
