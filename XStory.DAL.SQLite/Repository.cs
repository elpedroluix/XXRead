using SQLite;
using System;

namespace XStory.DAL.SQLite
{
    public class Repository
    {
        public SQLiteAsyncConnection SQLConnection { get; set; }

        public Repository()
        {

        }
    }
}
