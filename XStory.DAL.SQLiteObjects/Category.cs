using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.DAL.SQLiteObjects
{
    public class Category
    {
        public string Title { get; set; }
        [PrimaryKey]
        public string Url { get; set; }
        public bool IsEnabled { get; set; }
    }
}
