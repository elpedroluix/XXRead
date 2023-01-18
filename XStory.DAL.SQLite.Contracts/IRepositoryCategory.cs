using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.DAL.SQLite.Contracts
{
    public interface IRepositoryCategory
    {
        Task<List<SQLiteObjects.Category>> GetCategories();
        Task<List<SQLiteObjects.Category>> GetEnabledCategories();
        Task<SQLiteObjects.Category> GetCategory(string url);
        Task<int> InsertCategory(SQLiteObjects.Category category);
    }
}
