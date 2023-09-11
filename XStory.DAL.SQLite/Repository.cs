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
					this.BuildDatabase();
				}
				return _sqlConnection;
			}
		}

		private async void BuildDatabase()
		{
			await _sqlConnection.CreateTableAsync<SQLiteObjects.Story>();
			await _sqlConnection.CreateTableAsync<SQLiteObjects.Category>();
			await _sqlConnection.CreateTableAsync<SQLiteObjects.Author>();
		}

		public Repository()
		{

		}
	}
}
