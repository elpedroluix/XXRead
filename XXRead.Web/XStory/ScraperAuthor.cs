using HtmlAgilityPack;
using XXRead.Core.Models;

namespace XXRead.Web.XStory
{
	internal class ScraperAuthor
	{
		private RepositoryWeb _repositoryWeb;

		private ScraperStory _scraperStory;

		public const string AUTHOR_STATS_XPATH = "//ul[@class='ig-stats']/li[@class='ig-stat']";
		public const string AUTHOR_GENDER_XPATH = "//span[@class='ig-gender-part']";

		public ScraperAuthor(RepositoryWeb repositoryWeb, ScraperStory scraperStory)
		{
			_repositoryWeb = repositoryWeb;

			_scraperStory = scraperStory;
		}

		public async Task<Author> GetAuthorPage(Author author)
		{
			var uri = new Uri(author.Url);

			HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

			HtmlNode document = html.DocumentNode;

			// Infos
			this.GetAuthorInfos(author, document);

			// Is certified?
			this.GetAuthorCertified(author, document);

			// Stories
			await this.GetAuthorStories(author);

			return author;
		}

		private void GetAuthorCertified(Author author, HtmlNode document)
		{
			var certified = document.SelectSingleNode("//div[@id='badge']");
			author.IsCertified = certified != null;
		}

		private async Task GetAuthorStories(Author author)
		{
			author.Stories = await _scraperStory.GetAuthorStories(author.Url);

			if (author.Stories != null)
			{
				foreach (var story in author.Stories)
				{
					story.Author = author;
				}
			}
		}

		private void GetAuthorInfos(Author author, HtmlNode document)
		{
			// Gender
			var genderNode = document.SelectSingleNode(AUTHOR_GENDER_XPATH);
			if (genderNode != null)
			{
				author.Gender = genderNode.InnerText.Trim();
			}

			// Stats: histoires, chapitres, abonnés
			var statNodes = document.SelectNodes(AUTHOR_STATS_XPATH);
			if (statNodes != null)
			{
				foreach (var stat in statNodes)
				{
					string label = stat.SelectSingleNode("span")?.InnerText?.Trim().ToLowerInvariant() ?? string.Empty;
					string value = stat.SelectSingleNode("strong")?.InnerText?.Trim()
						?? stat.SelectSingleNode(".//strong")?.InnerText?.Trim()
						?? string.Empty;

					if (label.Contains("abonné") && !label.Contains("abonnement"))
					{
						author.FollowedBy = value;
					}
				}
			}
		}
	}
}
