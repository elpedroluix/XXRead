using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Web.Contracts
{
    public interface IServiceStory
    {
        Task<Story> GetStory(string path);
        /*Task<List<Story>> GetStoriesMainPage(int page, string sortCriterion);
        Task<List<Story>> GetFilteredStoriesMainPage(int page, List<string> hiddenCategories, string sortCriterion);
        Task<List<Story>> GetStoriesByCategory(int page = 0, string category = "", string sortCriterion = "");*/
        Task<List<Story>> GetStoriesPage(int page = 0, string category = "", string sortCriterion = "");
        List<Story> FilterStories(List<Story> stories, List<string> hiddenCategories);
    }
}
