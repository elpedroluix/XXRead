using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.DTO
{
    public class Story
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int ChapterNumber { get; set; }
        public string ChapterName { get; set; }
        public List<Story> ChaptersList { get; set; }
        public string Content { get; set; }
        public string ReleaseDate { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
        public string Url { get; set; }
        public long ViewsNumber { get; set; }
        public long LikesNumber { get; set; }
        public long ReviewsNumber { get; set; }
        public List<Post> Reviews {get; set;}
        public Author Author { get; set; }

        public Story()
        {
            ChaptersList = new List<Story>();
        }
    }
}
