using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.Logger
{
    public class Log
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string Content { get; set; }
        public string StackTrace { get; set; }
        public string Date { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
    }

    public enum LogType
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }
}
