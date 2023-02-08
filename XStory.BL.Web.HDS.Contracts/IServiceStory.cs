using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Web.HDS.Contracts
{
    public interface IServiceStory
    {
        Task<Story> GetStory(string path);
        Task<List<Story>> GetStoriesMainPage(int page);
        Task<List<Story>> GetStoriesByCategory(int page);
    }
}
