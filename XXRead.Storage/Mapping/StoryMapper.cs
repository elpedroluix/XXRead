using XXRead.Core.Models;

namespace XXRead.Storage.Mapping
{
	internal class StoryMapper
	{
		public static Story ToStoryDTO(Entities.Story story, Entities.Author? author)
		{
			return new Story()
			{
				Author = author == null ? null : AuthorMapper.ToAuthorDTO(author),
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

		public static Entities.Story ToStorySQLite(Story story)
		{
			return new Entities.Story()
			{
				AuthorUrl = story.Author?.Url,
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
