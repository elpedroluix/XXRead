using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XStory.Logger
{
    public class RepositoryLog
    {
        private SQLiteAsyncConnection _sqlConnection { get; set; }

        public SQLiteAsyncConnection SQLConnection
        {
            get
            {
                if (_sqlConnection == null)
                {
                    _sqlConnection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                    BuildDatabase();
                }
                return _sqlConnection;
            }
        }

        private async void BuildDatabase()
        {
            await SQLConnection.CreateTableAsync<Log>();
        }

        public RepositoryLog()
        {

        }

        public async Task<Log> GetLog(Guid id)
        {
            try
            {
                return await SQLConnection.Table<Log>().FirstAsync(l => l.Id == id);
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
