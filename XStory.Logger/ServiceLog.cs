using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XStory.Logger
{
    public class ServiceLog : RepositoryLog
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
