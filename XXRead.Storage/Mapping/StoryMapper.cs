using XXRead.Core.Models;

namespace XXRead.Storage.Mapping
{
	internal class StoryMapper
	{
		// TODO : Author is not restored (only AuthorId is stored), a favorite story comes back without its author
		public static Story ToStoryDTO(Entities.Story story)
		{
			return new Story()
			{
				CategoryName = story.CategoryName,
				CategoryUrl = story.CategoryUrl,
				ChapterName = story.ChapterName,
				ChapterNumber = story.ChapterNumber,
				Content = story.Content,
				Id = story.Id,
				LikesNumber = story.LikesNumber,
				ReleaseDate = story.ReleaseDate,
				ReviewsNumber = story.ReviewsNumber,
				Title = story.Title,
				Type = story.Type,
				Url = story.Url,
				ViewsNumber = story.ViewsNumber,
			};
		}

		// TODO : NullReferenceException when story.Author is null (always the case for XStory, see GetStory)
		public static Entities.Story ToStorySQLite(Story story)
		{
			return new Entities.Story()
			{
				AuthorId = story.Author.Id,
				CategoryName = story.CategoryName,
				CategoryUrl = story.CategoryUrl,
				ChapterName = story.ChapterName,
				ChapterNumber = story.ChapterNumber,
				Content = story.Content,
				Id = story.Id,
				LikesNumber = story.LikesNumber,
				ReleaseDate = story.ReleaseDate,
				ReviewsNumber = story.ReviewsNumber,
				Title = story.Title,
				Type = story.Type,
				Url = story.Url,
				ViewsNumber = story.ViewsNumber,
			};
		}
	}
}
