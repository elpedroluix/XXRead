using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStory.DTO;
using XStory.DTO.Config;
using XXRead.Core.Sources;

namespace XXRead.Tests.XStory
{
	[TestClass]
	public class AuthorTests
	{
		[TestMethod]
		public void GetAuthorPageTest_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Author author = new Author()
			{
				Id = "111900",
				Name = "Defalt",
				Avatar = "https://www.xstory-fr.com/forum/img/avatars/111900.jpg",
				Url = "auteur,111900,Defalt.html"
			};

			Task<Author> task = _source.GetAuthorPage(author);
			var result = task.Result;

			Assert.IsNotNull(result.Id);
		}

		[TestMethod]
		public void GetAuthorPageTest_FullInfos_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Author author = new Author()
			{
				Id = "215478",
				Name = "Lolo069",
				Avatar = "https://www.xstory-fr.com/forum/img/avatars/215478.png",
				Url = "https://www.xstory-fr.com/auteur,215478,Lolo069.html"
			};

			Task<Author> task = _source.GetAuthorPage(author);
			var result = task.Result;

			Assert.IsNotNull(result.Id);
		}

		[TestMethod]
		public void GetAuthorPageTest_NoStories_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Author author = new Author()
			{
				Id = "220448",
				Name = "Skell Dan",
				Avatar = "https://www.xstory-fr.com/forum/img/avatars/220448.png",
				Url = "https://www.xstory-fr.com/auteur,220488,Skell-Dan.html"
			};

			Task<Author> task = _source.GetAuthorPage(author);
			var result = task.Result;

			Assert.AreEqual(0, result.Stories.Count);
		}

		[TestMethod]
		public void GetAuthorPageTest_StoriesWithoutSubChapters_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Author author = new Author()
			{
				Id = "215478",
				Name = "Lolo069",
				Avatar = "https://www.xstory-fr.com/forum/img/avatars/215478.png",
				Url = "http://xstory-fr.com/auteur,215478,Lolo069.html"
			};

			Task<Author> task = _source.GetAuthorPage(author);
			var result = task.Result;

			Assert.AreNotEqual(result.Stories.Count, 0);
			Assert.AreEqual(0, result.Stories[1].ChaptersList.Count);
		}

		[TestMethod]
		public void GetAuthorPageTest_StoriesWithSubChapters_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Author author = new Author()
			{
				Id = "215478",
				Name = "Lolo069",
				Avatar = "https://www.xstory-fr.com/forum/img/avatars/215478.png",
				Url = "http://xstory-fr.com/auteur,215478,Lolo069.html"
				// https://www.xstory-fr.com/auteur,215478,Lolo069.html
			};

			Task<Author> task = _source.GetAuthorPage(author);
			var result = task.Result;

			Assert.AreNotEqual(result.Stories.Count, 0);
			Assert.AreEqual(result.Stories[0].ChaptersList.Count, 3);
		}

		[TestMethod]
		public void GetAuthorPageTest_NotCertified_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Author author = new Author()
			{
				Id = "119472",
				Name = "matchless",
				Avatar = "https://www.xstory-fr.com/forum/img/avatars/119472.jpg",
				Url = "https://www.xstory-fr.com/auteur,119472,matchless.html"
			};

			Task<Author> task = _source.GetAuthorPage(author);
			var result = task.Result;

			Assert.IsFalse(result.IsCertified);
		}

		[TestMethod]
		public void GetAuthorPageTestNoInfos_OK()
		{
			ISource _source = SourcesOnline.Get(DataSources.XStory);

			Author author = new Author()
			{
				Id = "220488",
				Name = "Skell Dan",
				Avatar = "https://www.xstory-fr.com/forum/img/avatars/220488.jpg",
				Url = "https://www.xstory-fr.com/auteur,220488,Skell-Dan.html"
			};

			Task<Author> task = _source.GetAuthorPage(author);
			var result = task.Result;

			Assert.IsNotNull(result.Id);
		}
	}
}
