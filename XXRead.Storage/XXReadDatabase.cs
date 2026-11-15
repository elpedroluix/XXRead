using SQLite;

namespace XXRead.Storage
{
	/// <summary>
	/// Singleton (see <see cref="DependencyInjection.AddXXReadStorage"/>) : one connection for the whole app.
	/// </summary>
	internal class XXReadDatabase
	{
		public SQLiteAsyncConnection Database;


		public XXReadDatabase()
		{
			Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
			Task.Run(this.InitDatabase).Wait();
		}

		private async Task InitDatabase()
		{
			await Database.CreateTableAsync<Entities.Author>();
			await Database.CreateTableAsync<Entities.Story>();
			await Database.CreateTableAsync<Entities.Category>();
		}

		public SQLiteAsyncConnection GetInstance()
		{
			return Database;
		}
	}
}
