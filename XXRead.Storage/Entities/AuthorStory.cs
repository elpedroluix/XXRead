using SQLite;

namespace XXRead.Storage.Entities
{
	[Table("AuthorStory")]
	internal class AuthorStory
	{
		[Indexed(Name = "CompositeKey", Order = 1, Unique = true)]
		public string AuthorId { get; set; }

		[Indexed(Name = "CompositeKey", Order = 2, Unique = true)]
		public string StoryId { get; set; }
	}
}
