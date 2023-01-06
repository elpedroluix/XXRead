using SQLite;
using System;

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
    }
}
