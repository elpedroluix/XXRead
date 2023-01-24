using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DAL.SQLite.Contracts;
using XStory.DAL.SQLiteObjects;

namespace XStory.DAL.SQLite
{
    public class RepositoryCategory : Repository, IRepositoryCategory
    {
        public async Task<List<Category>> GetCategories()
        {
            try
            {
                return await SQLConnection.Table<Category>().ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return null;
            }
        }

        public async Task<List<Category>> GetCategories(AsyncTableQuery<Category> query)
        {
            try
            {
                throw new NotImplementedException();
                //return await SQLConnection.QueryAsync().Table<Category>()..Where(query).ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return null;
            }
        }

        public async Task<Category> GetCategory(string url)
        {
            try
            {
                return await SQLConnection.Table<Category>().FirstOrDefaultAsync(c => c.Url == url);
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return null;
            }
        }

        public async Task<int> InsertCategory(Category category)
        {
            try
            {
                return await SQLConnection.InsertAsync(category);
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return -1;
            }
        }

        public async Task<int> InsertCategories(List<Category> categories)
        {
            try
            {
                return await SQLConnection.InsertAllAsync(categories, true);
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return -1;
            }
        }
    }
}
