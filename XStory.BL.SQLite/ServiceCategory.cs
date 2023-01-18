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

        public Task<List<Category>> GetCategories()
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetCategory(string url)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertCategory(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
