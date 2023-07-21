using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.DSLocator.Contracts;
using XStory.DTO;

namespace XStory.BL.Web.DSLocator
{
    public class ServiceCategory : IServiceCategory
    {
        private BL.Web.XStory.Contracts.IServiceCategory _serviceCategoryXStory;
        //private BL.Web.HDS.Contracts.IServiceCategory _serviceCategoryHDS;
        //private BL.Web.Demo.Contracts.IServiceCategory _serviceCategoryDemo;
        public ServiceCategory(BL.Web.XStory.Contracts.IServiceCategory serviceCategoryXStory)
        {
            _serviceCategoryXStory = serviceCategoryXStory;
        }

        public async Task<List<Category>> GetCategories(string dataSource)
        {
            try
            {
                switch (dataSource)
                {
                    case "XStory":
                        return await _serviceCategoryXStory.GetCategories();
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Logger.ServiceLog.Error(ex);
                return null;
            }
        }
    }
}
