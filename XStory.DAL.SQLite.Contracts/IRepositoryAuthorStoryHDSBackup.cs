using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.DAL.SQLite.Contracts
{
	public interface IRepositoryAuthorStoryHDSBackup
	{
		Task<int> InsertAuthorStory(DTO.Story story);
		Task<int> InsertAuthorStoryTransac(DTO.Story story);
	}
}
