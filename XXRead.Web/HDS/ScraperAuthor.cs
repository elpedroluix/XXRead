using HtmlAgilityPack;
using XStory.DTO;

namespace XXRead.Web.HDS
{
	internal class ScraperAuthor
	{
		private RepositoryWeb _repositoryWeb;

		private ScraperStory _scraperStory;

		public const string AUTHOR_INFOS_XPATH = "//div[contains(@class,'col-main')]//div[contains(@class,'user-content')]";
		public const string AUTHOR_STORIES_XPATH = "//div[contains(@class,'story-abstract-list')]/div";
		public const string AUTHOR_STORY_PAGINATION_XPATH = "//div[contains(@class,'col-main')]//div[contains(@class,'pagination')]";

		public ScraperAuthor(RepositoryWeb repositoryWeb, ScraperStory scraperStory)
		{
			_repositoryWeb = repositoryWeb;

			_scraperStory = scraperStory;
		}

		public async Task<Author> GetAuthorPage(Author author, int pageNumber = 1)
		{
			string uriPath = author.Url;
			if (pageNumber > 1)
			{
				uriPath += $"&p={pageNumber}";
			}

			var uri = new Uri(_repositoryWeb.BaseAddress, uriPath);

			HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

			HtmlNode document = html.DocumentNode;

			var authorInfosContainer = document.SelectSingleNode(AUTHOR_INFOS_XPATH);
			var authorStoriesContainer = document.SelectNodes(AUTHOR_STORIES_XPATH)
				.Where(node => node.Attributes["class"].Value == "story abstract");
			var authorPaginationContainer = document.SelectSingleNode(AUTHOR_STORY_PAGINATION_XPATH);

			// Infos
			// TODO : no get author infos everytime we get the page ? (once is enough)
			this.GetAuthorInfos(author, authorInfosContainer);

			// Stories
			this.GetAuthorStories(author, authorStoriesContainer);

			// Pages
			this.GetAuthorPagination(author, authorPaginationContainer);

			return author;
		}

		private void GetAuthorInfos(Author author, HtmlNode authorInfosContainer)
		{
			// Avatar
			if (string.IsNullOrWhiteSpace(author.Avatar))
			{
				var avatarNode = authorInfosContainer.SelectSingleNode(".//section[@class='avatar-section']/img");
				if (avatarNode != null)
				{
					author.Avatar = _repositoryWeb.BaseAddress + avatarNode.Attributes["src"].Value;
				}
			}

			// Gender
			var genderNode = authorInfosContainer.SelectSingleNode(".//section[@class='gender-section']");
			string genderText = genderNode?.InnerText ?? string.Empty;
			if (genderText.Contains("femme"))
			{
				author.Gender = "Femme";
			}
			else if (genderText.Contains("homme"))
			{
				author.Gender = "Homme";
			}
			else if (genderText.Contains("couple"))
			{
				author.Gender = "Couple";
			}
			else
			{
				author.Gender = "Non spécifié";
			}

			// Description & slogan
			var infoSections = authorInfosContainer.SelectNodes(".//div[@class='infos']/section");
			if (infoSections != null)
			{
				foreach (var sectionNode in infoSections)
				{
					if (sectionNode.SelectSingleNode("b")?.InnerText.Contains("Informations complémentaires :") ?? false)
					{
						author.Description = sectionNode.SelectSingleNode("pre")?.InnerText;
					}
					else if (sectionNode.SelectSingleNode("b")?.InnerText.Contains("Slogan de l'auteur :") ?? false)
					{
						author.Slogan = sectionNode.SelectSingleNode("pre")?.InnerText;
					}
				}
			}
		}

		/// <summary>
		/// Set or add stories to Author's stories list.
		/// </summary>
		/// <param name="author"></param>
		/// <param name="authorStoriesContainer"></param>
		private void GetAuthorStories(Author author, IEnumerable<HtmlNode> authorStoriesContainer)
		{
			var stories = _scraperStory.GetAuthorStories(authorStoriesContainer);

			if (author.Stories == null)
			{
				author.Stories = stories;
			}
			else
			{
				author.Stories.AddRange(stories);
			}

		}

		private void GetAuthorPagination(Author author, HtmlNode authorPaginationContainer)
		{
			var pagesList = authorPaginationContainer?.SelectNodes("ul/li") ?? null;
			if (pagesList != null)
			{
				if (pagesList.Last().HasClass("current"))
				{
					author.HasMorePages = false;
				}
				else
				{
					author.HasMorePages = true;
				}
			}
		}
	}
}
