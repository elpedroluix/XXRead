using System;
using System.Collections.Generic;
using System.Text;
using XStory.DTO;

namespace XStory.Helpers
{
    public static class StaticContext
    {
        public static List<Story> ListAlreadyLoadedStories = new List<Story>();

        public static string DATASOURCE;
    }
}
