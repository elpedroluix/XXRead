using XXRead.Core.Models;
using XXRead.Web.Demo;

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
			await GoldenMaster.Check(SOURCE, nameof(StoriesPage1), () => new SourceDemo().GetStoriesPage(0));
		}

		[TestMethod]
		public async Task Story1()
		{
			await GoldenMaster.Check(SOURCE, nameof(Story1), () => new SourceDemo().GetStory(""));
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

			await GoldenMaster.Check(SOURCE, nameof(AuthorPage), () => new SourceDemo().GetAuthorPage(author));
		}
	}
}
