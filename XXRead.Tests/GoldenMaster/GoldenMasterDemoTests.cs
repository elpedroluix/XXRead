using XStory.BL.Web.Demo;
using XStory.DTO;

namespace XXRead.Tests.GoldenMaster
{
	/// <summary>
	/// Demo source (no HTML), compared to the reference JSON (see <see cref="GoldenMaster"/>).
	/// </summary>
	[TestClass]
	public class GoldenMasterDemoTests
	{
		private const string SOURCE = "Demo";

		[TestMethod]
		public async Task StoriesPage1()
		{
			List<Story> result = await new ServiceStory().GetStoriesPage();

			GoldenMaster.Check(SOURCE, nameof(StoriesPage1), result);
		}

		[TestMethod]
		public async Task Story1()
		{
			Story result = await new ServiceStory().GetStory();

			GoldenMaster.Check(SOURCE, nameof(Story1), result);
		}

		[TestMethod]
		public async Task AuthorPage()
		{
			Author author = new Author()
			{
				Id = "987654",
				Name = "Lorem ipsum",
				Url = "auteur,123456,demo.html"
			};

			Author result = await new ServiceAuthor().GetAuthorPage(author);

			GoldenMaster.Check(SOURCE, nameof(AuthorPage), result);
		}
	}
}
