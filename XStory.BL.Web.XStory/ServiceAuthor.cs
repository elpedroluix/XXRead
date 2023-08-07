using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.XStory.Contracts;
using XStory.DAL.Web;
using XStory.DAL.Web.XStory.Contracts;
using XStory.DTO;
using XStory.Logger;

namespace XStory.BL.Web.XStory
{
	public class ServiceAuthor : IServiceAuthor
	{
		private IRepositoryWebXStory _repositoryWeb;

		public const string UNDEFINED = "Non précisé";

		public const string AUTHOR_INFOS_XPATH = "/html/body/div[1]/main/div[2]/section[1]/div/ul/li";
		public const string AUTHOR_STORIES_XPATH = "/html/body/div[1]/main/div[2]/section[2]/div/ul/li";

		public ServiceAuthor()
		{
			_repositoryWeb = new RepositoryWebXStory();
		}

		public async Task<Author> GetAuthorPage(Author author)
		{
			try
			{
				string authorId = author.Id;
				// Sets author name correctly for Uri. Ex : "author name" = "author-name"
				string authorName = author.Name.Trim().Replace(' ', '-');

				string authorUriPath = $"auteur,{authorId},{authorName}.html";
				var uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, authorUriPath);

				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

				HtmlNode document = html.DocumentNode;

				var authorInfosContainer = document.SelectNodes(AUTHOR_INFOS_XPATH);
				var authorStoriesContainer = document.SelectNodes(AUTHOR_STORIES_XPATH);

				// Infos
				this.GetAuthorInfos(author, authorInfosContainer);

				// Is certified ?
				this.GetAuthorCertified(author, document);

				// Stories
				await this.GetAuthorStories(author, authorStoriesContainer);


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

		private async Task GetAuthorStories(Author author, HtmlNodeCollection authorStoriesContainer)
		{
			IServiceStory serviceStory = new ServiceStory();

			author.Stories = await serviceStory.GetAuthorStories(author.Url);
		}

		private void GetAuthorInfos(Author author, HtmlNodeCollection authorInfosContainer)
		{
			int liCount = 0;

			// Status
			if (authorInfosContainer[liCount].SelectSingleNode("label").InnerText.Contains("Statut"))
			{
				string status = authorInfosContainer[liCount].SelectSingleNode("div")?.InnerText ?? null;
				author.Status = status;
			}
			else { author.Status = string.Empty; }
			liCount++;

			// Rank
			if (authorInfosContainer[liCount].SelectSingleNode("label").InnerText.Contains("Classement"))
			{
				string rank = authorInfosContainer[liCount].SelectSingleNode("div").InnerText;

				// Rank 30 days
				string rank30Days = rank.Split('(')[0].Trim();
				author.Rank30Days = rank30Days;

				// Rank all time
				string rankAllTime = rank
					.Split(new string[] { "classement" }, StringSplitOptions.RemoveEmptyEntries)[1]
					.Split('(')[0].Trim();
				author.RankAllTime = rankAllTime;
			}
			else
			{
				author.Rank30Days = string.Empty;
				author.RankAllTime = string.Empty;
			}
			liCount++;

			// Followed by
			if (authorInfosContainer[liCount].SelectSingleNode("label").InnerText.Contains("Suivi"))
			{
				string followedBy = string.Concat(
						authorInfosContainer[liCount].SelectSingleNode("div")?.InnerText ?? null,
						" ",
						authorInfosContainer[liCount].SelectSingleNode("div[3]")?.InnerText ?? null) ?? UNDEFINED;
				author.FollowedBy = followedBy;
			}
			else { author.FollowedBy = string.Empty; }
			liCount++;

			// Registrer date
			if (authorInfosContainer[liCount].SelectSingleNode("label").InnerText.Contains("inscription"))
			{
				string registerDate = authorInfosContainer[liCount].SelectSingleNode("div")?.InnerText?
				.Split(' ')[1] ?? null;
				author.RegisterDate = registerDate;
			}
			else { author.RegisterDate = string.Empty; }
			liCount++;

			// Gender
			if (authorInfosContainer[liCount].SelectSingleNode("label").InnerText.Contains("Genre"))
			{
				string gender = authorInfosContainer[liCount].SelectSingleNode("div")?.InnerText ?? UNDEFINED;
				author.Gender = gender;
			}
			else { author.Gender = string.Empty; }
			liCount++;

			// Age
			if (authorInfosContainer[liCount].SelectSingleNode("label").InnerText.Contains("Âge"))
			{
				string age = authorInfosContainer[liCount].SelectSingleNode("div")?.InnerText ?? UNDEFINED;
				author.Age = age;
			}
			else { author.Age = string.Empty; }
			liCount++;

			// Location
			if (authorInfosContainer[liCount].SelectSingleNode("label").InnerText.Contains("Lieu"))
			{
				string location = authorInfosContainer[liCount].SelectSingleNode("div/a")?.InnerText ?? UNDEFINED;
				author.Location = location;
			}
			else { author.Location = string.Empty; }
			//liCount++;

			// PLUS TARD // Contact
			//string contact = authorInfosContainer[liCount].SelectSingleNode("div")?.InnerText ?? null;
			//author.Contact = contact;
			//liCount++;
		}
	}
}
