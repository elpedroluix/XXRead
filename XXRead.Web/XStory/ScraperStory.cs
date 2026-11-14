using HtmlAgilityPack;
using System.Globalization;
using System.Text;
using XStory.DTO;
using XXRead.Web.XStory.Helpers;

namespace XXRead.Web.XStory
{
	internal class ScraperStory
	{
		private RepositoryWeb _repositoryWeb;

		private Uri _baseAdress;

		public const string HTML_BR = "br";
		public const string HTML_HREF = "href";

		public const string CHAPTER_TITLE_BEGIN = "Chapitre ";

		// Story list page — main grid (story-poster cards) and author profile page
		public const string STORIES_XPATH = "//div[contains(@class,'discover-stories__grid')]//a[contains(@class,'story-poster')]";
		public const string AUTHOR_PAGE_STORIES = "//div[contains(@class,'ig-posters')]//a[contains(@class,'story-poster')]";

		// Story / chapter page sections
		public const string STORY_HEADER_XPATH = "//div[contains(@class,'chapter-header')]";
		public const string STORY_CONTENT_XPATH = "//div[contains(@class,'chapter-body')]";
		public const string STORY_CHAPTERS_XPATH = "//div[@class='chapters-dropdown']//a[contains(@class,'xs-reading-title')]";

		public ScraperStory(RepositoryWeb repositoryWeb)
		{
			_repositoryWeb = repositoryWeb;
			_baseAdress = _repositoryWeb.BaseAddress;
		}

		public async Task<List<Story>> GetStoriesPage(int page = 0, string categoryUrl = "", string sortCriterion = "")
		{
			string baseUrl = string.IsNullOrWhiteSpace(categoryUrl)
				? string.Concat(_baseAdress, "histoires-erotiques")
				: categoryUrl;

			string queryString = string.Empty;
			bool hasQuery = false;

			if (!string.IsNullOrWhiteSpace(sortCriterion))
			{
				queryString += "?tri=" + sortCriterion;
				hasQuery = true;
			}

			if (page > 1)
			{
				queryString += (hasQuery ? "&" : "?") + "p=" + page;
			}

			Uri uri = new Uri(string.Concat(baseUrl, queryString));
			return await GetStoriesBase(uri);
		}

		public async Task<Story> GetStory(string storyUrl)
		{
			Story story = new Story();

			Uri uri = new Uri(storyUrl);

			HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

			HtmlNode document = html.DocumentNode;

			story.Url = uri.ToString();

			// HEADER container
			HtmlNode headerContainer = document.SelectSingleNode(STORY_HEADER_XPATH);

			// HEADER: author, type, date, views, likes, comments
			this.InitStoryInfos(story, headerContainer);

			// HEADER: book title + chapter number/name
			this.InitStoryTitleChapter(story, headerContainer);

			// HEADER: category
			this.InitStoryCategory(story, headerContainer);

			// AUTHOR avatar (JS-rendered, returns null)
			await this.SetStoryAuthorAvatar(story);

			// CONTENT
			HtmlNode storyContentContainer = document.SelectSingleNode(STORY_CONTENT_XPATH);
			this.InitStoryContent(story, storyContentContainer);

			// CHAPTERS LIST
			var storyChaptersListContainer = document.SelectNodes(STORY_CHAPTERS_XPATH);
			if (storyChaptersListContainer != null)
			{
				GetStorySubChapters(story, storyChaptersListContainer);
			}

			return story;
		}

		private async Task SetStoryAuthorAvatar(Story story)
		{
			if (story.Author == null) return;
			story.Author.Avatar = await this.GetAuthorAvatar(story.Author.Id);
		}

		private void InitStoryInfos(Story story, HtmlNode headerContainer)
		{
			if (headerContainer == null) return;

			// AUTHOR
			// TODO : author never found since the redesign, the header now has an <a class="xs-mention" data-show-user-details="..."> link (story.Author stays null)
			if (story.Author == null)
			{
				var authorNameNode = headerContainer.SelectSingleNode(
					".//div[contains(@class,'xs-username') and contains(@class,'name')]");
				if (authorNameNode != null)
				{
					string authorName = authorNameNode.InnerText.Trim();
					string authorId = authorNameNode.Attributes["data-show-user-details"]?.Value ?? string.Empty;
					story.Author = new Author()
					{
						Name = authorName,
						Id = authorId,
						Url = string.Concat(_baseAdress, "utilisateurs/", authorName.ToLower()),
					};
				}
			}

			// TYPE
			story.Type = headerContainer
				.SelectSingleNode(".//span[contains(@class,'label') and contains(@class,'type')]")
				?.InnerText?.Trim() ?? string.Empty;

			// RELEASE DATE
			if (string.IsNullOrWhiteSpace(story.ReleaseDate))
			{
				story.ReleaseDate = headerContainer
					.SelectSingleNode(".//div[@id='chapter-info-published-at']//time")
					?.Attributes["datetime"]?.Value ?? string.Empty;
			}

			// VIEWS
			string viewsText = headerContainer
				.SelectSingleNode(".//div[@id='chapter-info-view-count']//div[@class='sub']")
				?.InnerText?.Trim() ?? "0";
			story.ViewsNumber = ParseFormattedNumber(viewsText);

			// LIKES — data-value on the reactions counter element
			string likesValue = headerContainer
				.SelectSingleNode(".//*[contains(@class,'xs-reactions-count')]")
				?.Attributes["data-value"]?.Value ?? "0";
			story.LikesNumber = long.TryParse(likesValue, out long likes) ? likes : 0;

			// REVIEWS / COMMENTS
			string commentsValue = headerContainer
				.SelectSingleNode(".//*[contains(@class,'comments-count')]")
				?.Attributes["data-value"]?.Value ?? "0";
			story.ReviewsNumber = long.TryParse(commentsValue, out long comments) ? comments : 0;
		}

		private void InitStoryTitleChapter(Story story, HtmlNode headerContainer)
		{
			if (headerContainer == null) return;

			story.Title = headerContainer
				.SelectSingleNode(".//span[@class='book-title']")
				?.InnerText?.Trim() ?? string.Empty;

			string chapterText = headerContainer
				.SelectSingleNode(".//span[@class='chapter-title']")
				?.InnerText?.Trim() ?? string.Empty;

			ParseChapterTitle(story, chapterText);
		}

		private void ParseChapterTitle(Story story, string chapterText)
		{
			if (string.IsNullOrWhiteSpace(chapterText))
			{
				story.ChapterNumber = 1;
				return;
			}

			// Format: "Chapitre 1 · Le Majestic Sovereign"
			int separatorIndex = chapterText.IndexOf('·');
			if (separatorIndex > 0)
			{
				string numPart = chapterText.Substring(0, separatorIndex).Replace(CHAPTER_TITLE_BEGIN, "").Trim();
				story.ChapterNumber = int.TryParse(numPart, out int num) ? num : 1;
				story.ChapterName = chapterText.Substring(separatorIndex + 1).Trim();
			}
			else if (chapterText.StartsWith(CHAPTER_TITLE_BEGIN))
			{
				string numPart = chapterText.Replace(CHAPTER_TITLE_BEGIN, "").Trim();
				story.ChapterNumber = int.TryParse(numPart, out int num) ? num : 1;
				story.ChapterName = string.Empty;
			}
			else
			{
				story.ChapterNumber = 1;
				story.ChapterName = chapterText;
			}
		}

		private void InitStoryCategory(Story story, HtmlNode headerContainer)
		{
			if (headerContainer == null) return;

			var categoryNode = headerContainer.SelectSingleNode(".//a[contains(@class,'category')]");
			story.CategoryName = categoryNode?.InnerText?.Trim() ?? string.Empty;
			story.CategoryUrl = categoryNode?.Attributes[HTML_HREF]?.Value ?? string.Empty;
		}

		private void InitStoryContent(Story story, HtmlNode storyContentContainer)
		{
			if (storyContentContainer == null) return;

			var textNode = storyContentContainer.SelectSingleNode(".//*[@data-chapter-text]");
			if (textNode == null) return;

			var paragraphs = textNode.SelectNodes(".//div[contains(@class,'xs-paragraph')]");
			if (paragraphs == null) return;

			var sb = new StringBuilder();
			foreach (var para in paragraphs)
			{
				sb.Append(Environment.NewLine);
				AppendNodeText(para, sb);
			}

			story.Content = sb.ToString().TrimStart();
		}

		private void AppendNodeText(HtmlNode node, StringBuilder sb)
		{
			foreach (var child in node.ChildNodes)
			{
				if (child.Name == HTML_BR)
					sb.Append(Environment.NewLine);
				else if (child.NodeType == HtmlNodeType.Text)
					sb.Append(child.InnerText);
				else
					AppendNodeText(child, sb);
			}
		}

		private void GetStorySubChapters(Story story, HtmlNodeCollection storyChaptersListContainer)
		{
			foreach (var chapterLink in storyChaptersListContainer)
			{
				Story chapterStory = new Story();
				chapterStory.Author = story.Author;
				chapterStory.CategoryName = story.CategoryName;
				chapterStory.CategoryUrl = story.CategoryUrl;

				chapterStory.Url = chapterLink.Attributes[HTML_HREF]?.Value ?? string.Empty;

				string chapterText = chapterLink.InnerText.Trim();
				ParseChapterTitle(chapterStory, chapterText);

				chapterStory.Title = !string.IsNullOrWhiteSpace(chapterStory.ChapterName)
					? string.Concat(CHAPTER_TITLE_BEGIN, chapterStory.ChapterNumber, " : ", chapterStory.ChapterName)
					: string.Concat(CHAPTER_TITLE_BEGIN, chapterStory.ChapterNumber);

				story.ChaptersList.Add(chapterStory);
			}
		}

		private async Task<List<Story>> GetStoriesBase(Uri uri)
		{
			HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

			HtmlNode document = html.DocumentNode;
			var storiesContainer = document.SelectNodes(STORIES_XPATH);
			return this.GetStoriesFromContainer(storiesContainer);
		}

		private List<Story> GetStoriesFromContainer(HtmlNodeCollection storiesContainer, bool authorPage = false)
		{
			List<Story> stories = new List<Story>();
			if (storiesContainer == null) return stories;

			foreach (var container in storiesContainer)
			{
				Story story = new Story();

				// URL
				story.Url = container.Attributes[HTML_HREF]?.Value ?? string.Empty;

				// Title
				story.Title = container.SelectSingleNode(".//span[contains(@class,'story-poster__title')]")?.InnerText?.Trim()
					?? container.SelectSingleNode(".//span[contains(@class,'story-poster__heading')]")?.InnerText?.Trim()
					?? string.Empty;

				// Chapter subtitle
				story.ChapterName = container.SelectSingleNode(".//span[contains(@class,'story-poster__subheading')]")
					?.InnerText?.Trim() ?? string.Empty;

				// Author
				string authorName = container.SelectSingleNode(".//span[@class='story-poster__author-names']")
					?.InnerText?.Trim() ?? string.Empty;
				story.Author = new Author
				{
					Name = authorName,
					Url = string.Concat(_baseAdress, "utilisateurs/", authorName.ToLower()),
				};

				// Category — from category-swatches title attribute
				string categoryName = container.SelectSingleNode(".//span[contains(@class,'category-swatches')]")
					?.Attributes["title"]?.Value?.Trim() ?? string.Empty;
				story.CategoryName = categoryName;
				story.CategoryUrl = StaticUtils.GetCategoryUrl(categoryName);

				// Likes — from story-poster__likes title attribute ("2.3k J'aime")
				string likesTitle = container.SelectSingleNode(".//span[contains(@class,'story-poster__likes')]")
					?.Attributes["title"]?.Value ?? string.Empty;
				story.LikesNumber = ParseLikesFromTitle(likesTitle);

				// Release date
				story.ReleaseDate = container.SelectSingleNode(".//time")?.Attributes["datetime"]?.Value ?? string.Empty;

				stories.Add(story);
			}

			return stories;
		}

		private Task<string> GetAuthorAvatar(string authorId)
		{
			// Avatar is rendered via JavaScript web component — not available via static scraping
			return Task.FromResult<string>(null);
		}

		public async Task<List<Story>> GetAuthorStories(string authorPageUrl)
		{
			var uri = new Uri(authorPageUrl);
			HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

			HtmlNode document = html.DocumentNode;
			var storiesContainer = document.SelectNodes(AUTHOR_PAGE_STORIES);

			return this.GetStoriesFromContainer(storiesContainer, true);
		}

		private long ParseLikesFromTitle(string titleText)
		{
			if (string.IsNullOrWhiteSpace(titleText)) return 0;
			return ParseFormattedNumber(titleText.Split(' ')[0]);
		}

		private long ParseFormattedNumber(string text)
		{
			text = text?.Trim() ?? string.Empty;
			if (string.IsNullOrWhiteSpace(text)) return 0;

			if (text.EndsWith("k", StringComparison.OrdinalIgnoreCase))
			{
				string numPart = text.TrimEnd('k', 'K').Replace(',', '.');
				if (double.TryParse(numPart, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
					return (long)(val * 1000);
			}

			string noSpaces = string.Concat(text.Split(' '));
			if (long.TryParse(noSpaces, out long result)) return result;
			return 0;
		}
	}
}
