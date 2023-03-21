using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.DAL.SQLiteObjects
{
    [Table("Author")]
    public class Author
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
