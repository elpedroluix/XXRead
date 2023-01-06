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
        public int Severity { get; set; }
    }
}
