using System;
using System.Collections.Generic;
using System.Text;
using XStory.DTO;

namespace XStory.Helpers
{
    public static class StaticContext
    {
        public const string BASE_URL = "https://www.xstory-fr.com/";
        public static List<Story> ListAlreadyLoadedStories = new List<Story>();
    }
}
