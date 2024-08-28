using SQLite;
using System;
using System.Threading.Tasks;
using XStory.DAL.SQLite.Contracts;

namespace XStory.DAL.SQLite
{
	public class RepositoryHDSBackup
	{
		protected IXXReadDatabaseHDSBackup _database;

		public RepositoryHDSBackup(IXXReadDatabaseHDSBackup database)
		{
			_database = database;
		}
	}
}
