using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.XStory.Contracts;
using XStory.DAL.Web.XStory;
using XStory.DAL.Web.XStory.Contracts;
using XStory.DTO;
using XStory.Logger;

namespace XStory.BL.Web.XStory
{
	public class ServiceAuthor : IServiceAuthor
	{
		private IRepositoryWebXStory _repositoryWeb;

		public const string AUTHOR_STATS_XPATH = "//ul[@class='ig-stats']/li[@class='ig-stat']";
		public const string AUTHOR_GENDER_XPATH = "//span[@class='ig-gender-part']";

		public ServiceAuthor()
		{
			_repositoryWeb = new RepositoryWebXStory();
		}

		public async Task<Author> GetAuthorPage(Author author)
		{
			try
			{
				var uri = new Uri(author.Url);

				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

				HtmlNode document = html.DocumentNode;

				// Infos
				this.GetAuthorInfos(author, document);

				// Is certified?
				this.GetAuthorCertified(author, document);

				// Stories
				await this.GetAuthorStories(author);

				return author;
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return author;
			}
		}

		private void GetAuthorCertified(Author author, HtmlNode document)
		{
			var certified = document.SelectSingleNode("//div[@id='badge']");
			author.IsCertified = certified != null;
		}

		private async Task GetAuthorStories(Author author)
		{
			IServiceStory serviceStory = new ServiceStory();

			author.Stories = await serviceStory.GetAuthorStories(author.Url);

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
