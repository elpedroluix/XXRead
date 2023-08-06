using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Web.DSLocator.Contracts
{
    public interface IServiceStory
    {
        Task<Story> GetStory(string dataSource, string path);
        Task<List<Story>> GetStoriesPage(string dataSource, int page = 0, string category = "", string sortCriterion = "");
        List<Story> FilterStories(string dataSource, List<Story> stories, List<string> hiddenCategories);
        Task<string> GetAuthorAvatar(string dataSource, string authorId);
    }
}
