using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.DAL.SQLite.Contracts
{
    public interface IRepositoryCategory
    {
        Task<List<SQLiteObjects.Category>> GetCategories();
        Task<List<SQLiteObjects.Category>> GetCategories(AsyncTableQuery<SQLiteObjects.Category> query);
        Task<SQLiteObjects.Category> GetCategory(string url);
        Task<int> Save(SQLiteObjects.Category category);
        Task<int> InsertCategory(SQLiteObjects.Category category);
        Task<int> InsertCategories(List<SQLiteObjects.Category> categories);
    }
}
