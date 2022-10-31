using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.DAL.SQLite.Contracts
{
    internal interface IRepositorySettings : IRepository
    {
        List<DTO.Setting> GetSettings();
    }
}
