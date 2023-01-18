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
                return null;
            }
        }

        public async Task<List<Category>> GetEnabledCategories()
        {
            try
            {
                return await SQLConnection.Table<Category>().Where(c => c.IsEnabled).ToListAsync();
            }
            catch (Exception ex)
            {
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
                return 1;
            }
        }
    }
}
