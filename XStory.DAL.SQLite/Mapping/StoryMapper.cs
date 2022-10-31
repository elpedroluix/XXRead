using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.DAL.SQLite.Mapping
{
    public class StoryMapper
    {
        public static DTO.Story ToStoryDTO(SQLiteObjects.Story story)
        {
            return new DTO.Story()
            {
                Url = story.Url,
                Title = story.Title
            };
        }

        public static SQLiteObjects.Story ToStorySQLite(DTO.Story story)
        {
            return new SQLiteObjects.Story()
            {
                Title = story.Title,
                Url = story.Url
            };
        }
    }
}
