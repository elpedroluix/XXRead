using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Common.Contracts
{
	public interface IServiceStory
	{
		Task<List<DTO.Story>> InitStories();
		Task<List<DTO.Story>> LoadMoreStories();
		Task<List<DTO.Story>> RefreshStories(DTO.Story firstStory);
		List<DTO.Story> DistinctStories(List<DTO.Story> stories);
		void SetPageNumber(int value);
		void IncrementPageNumber();
		void ResetPageNumber();
	}
}