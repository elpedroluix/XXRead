using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.DTO
{
    public class Post
    {
        public User User { get; set; }
        public string Content { get; set; }
        public string Signature { get; set; }
        public string Date { get; set; }
    }
}
