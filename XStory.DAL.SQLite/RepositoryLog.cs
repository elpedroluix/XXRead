using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DAL.SQLite.Contracts;
using XStory.DAL.SQLiteObjects;

namespace XStory.DAL.SQLite
{
    public class RepositoryLog : Repository, IRepositoryLog
    {
        public async Task<Log> GetLog(Guid id)
        {
            try
            {
                return await SQLConnection.Table<Log>().FirstOrDefaultAsync(l => l.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Log>> GetLogs()
        {
            try
            {
                return await SQLConnection.Table<Log>().ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> InsertLog(Log log)
        {
            try
            {
                return await SQLConnection.InsertAsync(log);
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
}
