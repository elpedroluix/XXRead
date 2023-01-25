using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.SQLite.Contracts;
using XStory.DAL.SQLite;
using XStory.DAL.SQLite.Contracts;
using XStory.DTO;

namespace XStory.BL.SQLite
{
    public class ServiceCategory : IServiceCategory
    {
        private IRepositoryCategory _repositoryCategory;

        public ServiceCategory()
        {
            _repositoryCategory = new RepositoryCategory();
        }

        public async Task<bool> HasDBCategories()
        {
            try
            {
                List<XStory.DAL.SQLiteObjects.Category> categories = await _repositoryCategory.GetCategories();
                if (categories == null || categories.Count == 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return false;
            }
        }

        public async Task<int> Save(Category category)
        {
            try
            {
                return await _repositoryCategory.Save(new DAL.SQLiteObjects.Category()
                {
                    Title = category.Title,
                    Url = category.Url,
                    IsEnabled = category.IsEnabled
                });
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return -1;
            }
        }

        public async Task<List<Category>> GetCategories()
        {
            try
            {
                List<XStory.DAL.SQLiteObjects.Category> _sqliteCategories = await _repositoryCategory.GetCategories();

                List<DTO.Category> categories = new List<DTO.Category>();

                _sqliteCategories.ForEach(categ =>
                {

                    categories.Add(new Category()
                    {
                        Title = categ.Title,
                        Url = categ.Url,
                        IsEnabled = categ.IsEnabled
                    });
                });
                return categories;
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
                XStory.DAL.SQLiteObjects.Category _sqliteCategory = await _repositoryCategory.GetCategory(url);

                return new DTO.Category()
                {
                    Title = _sqliteCategory.Title,
                    Url = _sqliteCategory.Url,
                    IsEnabled = _sqliteCategory.IsEnabled
                };
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
                return await _repositoryCategory.InsertCategory(new DAL.SQLiteObjects.Category()
                {
                    Title = category.Title,
                    Url = category.Url,
                    IsEnabled = category.IsEnabled
                });
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
                List<DAL.SQLiteObjects.Category> sqliteCategories = new List<DAL.SQLiteObjects.Category>();

                categories.ForEach(categ =>
                {
                    sqliteCategories.Add(new DAL.SQLiteObjects.Category()
                    {
                        Title = categ.Title,
                        Url = categ.Url,
                        IsEnabled = categ.IsEnabled
                    });
                });

                await _repositoryCategory.InsertCategories(sqliteCategories);
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Log("Error", ex.Message, ex.Source, DateTime.Now, Logger.LogType.Error);
                return -1;
            }
            return 0;
        }
    }
}
