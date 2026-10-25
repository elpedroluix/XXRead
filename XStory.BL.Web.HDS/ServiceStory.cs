using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XStory.BL.Web.HDS.Contracts;
using XStory.BL.Web.HDS.Helpers;
using XStory.DAL.Web.HDS;
using XStory.DAL.Web.HDS.Contracts;
using XStory.DTO;
using XStory.Logger;

namespace XStory.BL.Web.HDS
{
	public class ServiceStory : IServiceStory
	{
		private IRepositoryWebHDS _repositoryWeb;

		public const string STORIES_XPATH = "//div[contains(@class,'story-abstract-list')]/div";

		public const string STORY_CONTAINER_XPATH = "//div[@class='main-column']";
		public const string STORY_CONTENT_XPATH = "//div[@class='main-column']//div[@class='story']/div[@class='text']";

		public const string AVATAR_URL = "avatars/6265.jpg";
		public const string UNKNOWN_AUTHOR_AVATAR = "unknown_author_avatar";

		public const string IMAGE_XPATH = "/html/body/img";

		public ServiceStory()
		{
			_repositoryWeb = new RepositoryWebHDS();
		}

		public async Task<Story> GetStory(string path)
		{
			Story story = new Story();
			try
			{
				Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, path);

				// Whole page
				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

				HtmlNode document = html.DocumentNode;

				story.Url = uri.ToString();

				// HEADER infos
				HtmlNode storyContainer = document.SelectSingleNode(STORY_CONTAINER_XPATH);
				this.InitStoryInfos(story, storyContainer);

				// HEADER Author avatar
				this.SetStoryAuthorAvatar(story);

				// CONTENT
				HtmlNode storyContentContainer = document.SelectSingleNode(STORY_CONTENT_XPATH);
				this.InitStoryContent(story, storyContentContainer);
			}
			catch (Exception ex)
			{
				story = null;
				Logger.ServiceLog.Error(ex);
			}
			return story;
		}

		private void InitStoryInfos(Story story, HtmlNode storyContainer)
		{
			// TITLE
			var titleNode = storyContainer.SelectSingleNode(".//h2[@class='title']");
			if (titleNode != null)
			{
				story.Title = titleNode.InnerText;
			}

			// AUTHOR
			if (story.Author == null)
			{
				string authorAvatar = string.Empty;

				var avatarNode = storyContainer.SelectSingleNode(".//div[@class='left-part']/img[@class='avatar']");
				if (avatarNode?.Attributes["src"] != null)
				{
					authorAvatar = avatarNode.Attributes["src"].Value;
				}

				var authorNode = storyContainer.SelectSingleNode(".//div[@class='right-part']/div[1]/a");
				if (authorNode?.Attributes["href"] != null)
				{
					string authorUrl = authorNode.Attributes["href"].Value;
					string authorName = authorUrl.Split('=')[1];
					Author author = new Author()
					{
						Name = authorName,
						Url = authorUrl,
						Avatar = authorAvatar,
					};
					story.Author = author;
				}
			}

			// PUBLISHED DATE & CATEGORY
			var publishNode = storyContainer.SelectSingleNode(".//div[@class='published_at']");
			if (publishNode != null)
			{
				string publishInfo = publishNode.InnerText.Trim();
				//Histoire érotique Publiée sur HDS le 07-02-2023 dans la catégorie Entre-nous, hommes et femmes
				publishInfo = publishInfo.Replace("Histoire érotique Publiée sur HDS le ", "");
				//07-02-2023 dans la catégorie Entre-nous, hommes et femmes

				// RELEASE DATE
				if (string.IsNullOrWhiteSpace(story.ReleaseDate))
				{
					string releaseDate = publishInfo.Remove(10);
					story.ReleaseDate = releaseDate;
				}

				if (string.IsNullOrWhiteSpace(story.CategoryName))
				{
					string category = publishInfo.Replace(" dans la catégorie ", "");
					category = string.Concat(category.Skip(10));
					story.CategoryName = category;

					if (Helpers.StaticUtils.StoryCategoryUrlDictionary.ContainsKey(category))
						story.CategoryUrl = Helpers.StaticUtils.StoryCategoryUrlDictionary[category];
				}
			}

			// VIEWS
			var viewsNode = storyContainer.SelectSingleNode(".//div[@class='visit_count']/b");
			if (viewsNode != null)
			{
				string viewsCount = viewsNode.InnerText;
				story.ViewsNumber = long.Parse(string.Concat(viewsCount.Split(' ')));
			}
		}

		private void SetStoryAuthorAvatar(Story story)
		{
			string avatar = this.GetAuthorAvatar(story.Author.Avatar);
			story.Author.Avatar = avatar;
		}

		private void InitStoryContent(Story story, HtmlNode storyContentContainer)
		{
			// CONTENT
			string content = storyContentContainer.InnerText.Replace("<br>", Environment.NewLine);
			story.Content = content;
		}

		private async Task<List<Story>> GetStoriesBase(Uri uri)
		{
			try
			{
				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

				HtmlNode document = html.DocumentNode;
				var storiesContainer = document.SelectNodes(STORIES_XPATH).Where(node => node.Attributes["class"].Value == "story abstract");

				List<Story> stories = this.GetStoriesFromContainer(storiesContainer);

				return stories;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message + Environment.NewLine + ex.InnerException);
			}
			return null;
		}

		private List<Story> GetStoriesFromContainer(IEnumerable<HtmlNode> storiesContainer)
		{
			List<Story> stories = new List<Story>();
			foreach (var container in storiesContainer)
			{
				var metaSpans = container.SelectNodes(".//div[@class='meta']/span[@class='meta-item']");
				if (metaSpans == null || metaSpans.Count < 3) continue;

				var authorNode = container.SelectSingleNode(".//a[@class='user-link']");
				var titleNode = container.SelectSingleNode(".//div[@class='title']/a[@class='story-link']");

				Story story = new Story();

				// RELEASE DATE
				// "Histoire érotique publiée le 07-02-2023 à 09 h"
				string releaseDate = metaSpans[0].InnerText.Trim();
				releaseDate = releaseDate.Replace("Histoire érotique publiée le ", "");
				// "07-02-2023 à 09 h"
				releaseDate = releaseDate.Replace(" à ", " ");
				// "07-02-2023 09 h"
				releaseDate = releaseDate.Replace(" h", ":00:00");
				// "07-02-2023 09:00:00"
				story.ReleaseDate = releaseDate;

				// TITLE
				story.Title = titleNode?.InnerText;

				// URL
				story.Url = titleNode?.Attributes["href"]?.Value;

				// CATEGORY
				string category = metaSpans[1].InnerText.Trim();
				if (!string.IsNullOrEmpty(category))
				{
					story.CategoryName = category;
					if (Helpers.StaticUtils.StoryCategoryUrlDictionary.ContainsKey(category))
						story.CategoryUrl = Helpers.StaticUtils.StoryCategoryUrlDictionary[category];
				}

				// AUTHOR
				Author author = new Author();
				if (authorNode != null)
				{
					author.Url = authorNode.Attributes["href"].Value;
					author.Name = authorNode.InnerText;
				}
				story.Author = author;

				stories.Add(story);
			}

			return stories;
		}

		public async Task<List<Story>> GetStoriesPage(int page = 0, string categoryUrl = "", string sortCriterion = "")
		{
			try
			{
				string basePath = "/sexe/";
				string categoryPath = string.Empty;
				string pagePath = page > 1 ? "?p=" + page : "";
				string extensionPath = ".php";

				if (!string.IsNullOrWhiteSpace(categoryUrl))
				{
					categoryPath = StaticUtils.CategoryFromUrlDictionary[categoryUrl];
				}
				else
				{
					categoryPath = "histoires-par-date";
				}

				Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress,
					string.Concat(basePath, categoryPath, extensionPath, pagePath, sortCriterion));
				return await GetStoriesBase(uri);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return null;
		}

		public List<Story> FilterStories(List<Story> stories, List<string> hiddenCategories)
		{
			if (hiddenCategories != null && hiddenCategories.Count > 0)
			{
				stories = stories.Where(story =>
				{
					bool isValid = true;
					foreach (var category in hiddenCategories)
					{
						if (story.CategoryUrl == category)
						{
							isValid = false;
							continue;
						}
					}
					return isValid;
				}).ToList();
			}

			return stories;
		}

		public string GetAuthorAvatar(string authorAvatar)
		{
			try
			{
				return string.Concat(_repositoryWeb.GetHttpClient().BaseAddress, authorAvatar);
			}
			catch (Exception ex)
			{
				ServiceLog.Error(ex);
				return UNKNOWN_AUTHOR_AVATAR;
			}
		}

		public List<Story> GetAuthorStories(IEnumerable<HtmlNode> authorStoriesContainer)
		{
			var stories = this.GetStoriesFromContainer(authorStoriesContainer);

			return stories;
		}
	}
}
