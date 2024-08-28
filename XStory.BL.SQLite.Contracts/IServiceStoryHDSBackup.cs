using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.BL.SQLite.Contracts
{
	public interface IServiceStoryHDSBackup : IServiceStory
	{
		Task<List<DTO.Story>> GetStories(int startIndex = 0, int endIndex = 0);
	}
}
