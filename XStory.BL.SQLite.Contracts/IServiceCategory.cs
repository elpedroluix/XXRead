using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XStory.BL.SQLite.Contracts
{
    public interface IServiceCategory
    {
        Task<bool> HasDBCategories();
        Task<List<DTO.Category>> GetCategories();
        Task<DTO.Category> GetCategory(string url);
        Task<int> Save(DTO.Category category);
        Task<int> InsertCategory(DTO.Category category);
        Task<int> InsertCategories(List<DTO.Category> categories);
    }
}
