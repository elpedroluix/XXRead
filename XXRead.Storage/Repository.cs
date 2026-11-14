namespace XXRead.Storage
{
	/// <summary>
	/// The repositories throw on error : the Core services log and handle them.
	/// </summary>
	internal class Repository
	{
		protected XXReadDatabase _database;

		public Repository(XXReadDatabase database)
		{
			_database = database;
		}
	}
}
