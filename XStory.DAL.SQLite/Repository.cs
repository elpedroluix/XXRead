using SQLite;
using System;

namespace XStory.DAL.SQLite
{
    public class Repository
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
            //await SQLConnection.CreateTableAsync<SQLiteObjects.Setting>();
            await SQLConnection.CreateTableAsync<SQLiteObjects.Story>();
            await SQLConnection.CreateTableAsync<SQLiteObjects.Category>();
        }

        public Repository()
        {

        }
    }
}
