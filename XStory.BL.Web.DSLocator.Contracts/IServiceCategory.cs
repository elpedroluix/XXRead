using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;

namespace XStory.BL.Web.DSLocator.Contracts
{
    public interface IServiceCategory
    {
        Task<List<DTO.Category>> GetCategories(string dataSource);
    }
}
