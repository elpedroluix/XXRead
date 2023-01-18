using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.DAL.SQLiteObjects
{
    public class Log
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string Content { get; set; }
        public int Severity { get; set; }
    }
}
