using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.DAL.SQLite.Contracts
{
	public interface IRepositoryStoryHDSBackup
	{
		Task<List<DTO.Story>> GetStories();
		Task<List<DTO.Story>> GetStories(string query = "");
		Task<DTO.Story> GetStory(string url);
		Task<int> InsertStory(DTO.Story story);
		Task<int> DeleteStory(DTO.Story story);
	}
}
