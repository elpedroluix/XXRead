using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.BL.SQLite.Contracts
{
    public interface IServiceCategory
    {
        Task<List<DTO.Category>> GetCategories();
        Task<DTO.Category> GetCategory(string url);
        Task<int> InsertCategory(DTO.Category category);
    }
}
