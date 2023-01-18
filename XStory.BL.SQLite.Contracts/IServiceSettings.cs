using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XStory.BL.SQLite.Contracts
{
    public interface IServiceSettings
    {
        Task<List<DTO.Setting>> GetSettings();
        Task<DTO.Setting> GetSetting(Guid id);
        Task<DTO.Setting> GetSetting(string key);
        Task<int> InsertSetting(DTO.Setting setting);
    }
}
