using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Web.Contracts
{
    public interface IServiceStory
    {
        Task<Story> GetStory(string path);
        Task<List<Story>> GetStoriesMainPage(int page, string sortCriterion);
        Task<List<Story>> GetFilteredStoriesMainPage(int page, string[] hiddenCategories, string sortCriterion);
        Task<List<Story>> GetStoriesByCategory(int page, string sortCriterion);
    }
}
