﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.Models
{
    public class Story
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Chapter { get; set; }
        public string ChapterName { get; set; }
        public List<string> ChaptersList { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }

        /*public List<Reviews> Reviews {get; set;}*/
        public Story()
        {
            ChaptersList = new List<string>();
        }
    }
}
