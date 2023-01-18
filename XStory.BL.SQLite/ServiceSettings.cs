using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.DAL.SQLite.Contracts;
using XStory.DTO;

namespace XStory.BL.SQLite
{
    public class ServiceSettings : BL.SQLite.Contracts.IServiceSettings
    {
        private IRepository _repositorySQLite;

        public Task<Setting> GetSetting(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Setting> GetSetting(string key)
        {
            throw new NotImplementedException();
        }

        public Task<List<Setting>> GetSettings()
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertSetting(Setting setting)
        {
            throw new NotImplementedException();
        }
    }
}
