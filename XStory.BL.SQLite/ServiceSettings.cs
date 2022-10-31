using System;
using XStory.DAL.SQLite.Contracts;

namespace XStory.BL.SQLite
{
    public class ServiceSettings : BL.SQLite.Contracts.IServiceSettings
    {
        private IRepository _repositorySQLite;
    }
}
