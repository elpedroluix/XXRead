using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Web.Demo.Contracts
{
    public interface IServiceStory
    {
        Task<Story> GetStory(string path);
        Task<List<Story>> GetStoriesPage(int page = 0, string category = "", string sortCriterion = "");
        List<Story> FilterStories(List<Story> stories, List<string> hiddenCategories);
		Task<string> GetAuthorAvatar(string authorId);
	}
}
