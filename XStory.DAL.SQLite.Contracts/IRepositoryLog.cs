using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DAL.SQLiteObjects;

namespace XStory.DAL.SQLite.Contracts
{
    public interface IRepositoryLog
    {
        Task<List<SQLiteObjects.Log>> GetLogs();
        Task<SQLiteObjects.Log> GetLog(Guid id);
        Task<int> InsertLog(SQLiteObjects.Log log);
    }
}
