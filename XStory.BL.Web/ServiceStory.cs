using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.BL.Web.Contracts;
using XStory.DAL.Web;
using XStory.DAL.Web.Contracts;
using XStory.DTO;

namespace XStory.BL.Web
{
    public class ServiceStory : IServiceStory
    {
        private IRepositoryWeb _repositoryWeb;

        public const string STORIES_XPATH = "/html/body/div[1]/main/section[4]/div[2]/ul/li/div";

        public const string STORY_HEADER_XPATH = "/html/body/div[1]/main/div[1]/section[1]";
        public const string STORY_CONTENT_XPATH = "/html/body/div[1]/main/div[1]/section[2]";
        public const string STORY_FOOTER_XPATH = "/html/body/div[1]/main/div[1]/section[3]";

        public const string STORY_CHAPTERS_XPATH = "/html/body/div[1]/aside/div[1]/section[1]/div/ul/li";


        public ServiceStory()
        {
            _repositoryWeb = new RepositoryWeb();
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

                // head
                HtmlNode storyHeaderContainer = document.SelectSingleNode(STORY_HEADER_XPATH);
                if (story.Author == null)
                {
                    Author author = new Author()
                    {
                        Id = storyHeaderContainer.SelectSingleNode("div/div[1]/ul/li[1]/a").Attributes["data-author-id"].Value,
                        Name = storyHeaderContainer.SelectSingleNode("div/div[1]/ul/li[1]/a").InnerHtml,
                        Url = storyHeaderContainer.SelectSingleNode("div/div[1]/ul/li[1]/a").Attributes["href"].Value
                    };
                    story.Author = author;
                }
                if (string.IsNullOrWhiteSpace(story.ReleaseDate))
                {
                    story.ReleaseDate = storyHeaderContainer.SelectSingleNode("div/div[1]/ul/li[3]/time").Attributes["datetime"].Value;
                }
                story.ViewsNumber = long.Parse(string.Concat(storyHeaderContainer.SelectSingleNode("div/div[1]/ul/li[4]/span").InnerHtml.Split(' ')));
                story.LikesNumber = long.Parse(storyHeaderContainer.SelectSingleNode("div/div[1]/ul/li[5]/a/span").InnerHtml);
                story.ReviewsNumber = long.Parse(storyHeaderContainer.SelectSingleNode("div/div[1]/ul/li[6]/a").InnerHtml.Split(' ')[0]);

                story.Title = storyHeaderContainer.SelectSingleNode("div/div[3]/h1").InnerHtml;
                story.Url = uri.ToString();
                string chapterTryNumber = storyHeaderContainer.SelectSingleNode("div/div[3]/h2[1]").InnerHtml.Split(' ')[1];
                story.ChapterNumber = chapterTryNumber == "unique" ? 1 : int.Parse(chapterTryNumber);
                // Chapter number ("Chapitre x")
                story.ChapterName = storyHeaderContainer.SelectSingleNode("div/div[3]/h2[1]").InnerHtml;
                // Chapter name
                if (storyHeaderContainer.SelectSingleNode("div/div[3]/h2[2]") != null)
                {
                    story.ChapterName += String.Concat(" : ", storyHeaderContainer.SelectSingleNode("div/div[3]/h2[2]")?.InnerHtml ?? string.Empty);
                }


                story.CategoryName = storyHeaderContainer.SelectSingleNode("div/div[3]/a/div[2]").InnerHtml;
                story.CategoryUrl = storyHeaderContainer.SelectSingleNode("div/div[3]/a").Attributes["href"].Value;

                HtmlNode storyContentContainer = document.SelectSingleNode(STORY_CONTENT_XPATH);
                var storyContentParagraphsContainer = storyContentContainer.SelectNodes("div").Nodes();

                string storyContent = string.Empty;
                foreach (var element in storyContentParagraphsContainer)
                {
                    if (element.Name == "p:p")
                    {
                        if (element.Attributes["class"].Value == "xs-lire-histoire-paragraphe")
                        {
                            if (element.InnerLength == 1)
                            {
                                storyContent += Environment.NewLine + Environment.NewLine;
                            }
                            else
                            {
                                storyContent += Environment.NewLine + element.InnerText;
                            }
                        }

                    }
                    else if (element.Name == "br")
                    {
                        storyContent += Environment.NewLine + Environment.NewLine;
                    }
                }
                story.Content = storyContent;

                // chapters list
                var storyChaptersList = document.SelectNodes(STORY_CHAPTERS_XPATH);

                if (storyChaptersList != null)
                {
                    GetStoryChapters(story, storyChaptersList);
                }

            }
            catch (Exception e)
            {
                story = null;
                XStory.Logger.ServiceLog.Log("Error", e.Message, e.Source, DateTime.Now, Logger.LogType.Error);
            }
            return story;
        }

        private void GetStoryChapters(Story story, HtmlNodeCollection storyChaptersList)
        {
            foreach (var storyChapter in storyChaptersList)
            {
                Story chapterStory = new Story();
                chapterStory.Author = story.Author;

                // TODO :
                // - date
                chapterStory.ReleaseDate = storyChapter.SelectSingleNode("time").Attributes["datetime"].Value;
                // - chapter category
                chapterStory.CategoryName = Helpers.StaticUtils.CategoryNameDictionary[storyChapter.SelectSingleNode("a/i").Attributes["class"].Value.Split(' ')[1]];
                // - chapter number
                chapterStory.ChapterNumber = int.Parse(storyChapter.SelectSingleNode("a").InnerText.Split(' ')[2]);
                // - chapter name
                chapterStory.ChapterName = storyChapter.SelectSingleNode("a").Attributes["title"]?.Value ?? string.Empty;

                // Beautify display chapter title
                if (chapterStory.ChapterNumber > 0)
                {
                    if (!string.IsNullOrWhiteSpace(chapterStory.ChapterName))
                    {
                        chapterStory.Title = string.Concat("Chapitre ", chapterStory.ChapterNumber, " : ", chapterStory.ChapterName);
                    }
                    else
                    {
                        chapterStory.Title = string.Concat("Chapitre ", chapterStory.ChapterNumber);
                    }
                }
                // - likes
                chapterStory.LikesNumber = long.Parse(storyChapter.SelectSingleNode("div/span").InnerText);
                // - url
                chapterStory.Url = storyChapter.SelectSingleNode("a").Attributes["href"]?.Value ?? string.Empty;

                story.ChaptersList.Add(chapterStory);
            }
        }

        public Task<List<Story>> GetStoriesByCategory(int page, string sortCriterion)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Story>> GetStoriesMainPage(int page = 0, string sortCriterion = "")
        {
            try
            {
                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, string.Concat("/histoires-erotiques", (page > 1 ? ",,," + page : ""), sortCriterion, ".html"));
                return await GetStoriesBase(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        private async Task<List<Story>> GetStoriesBase(Uri uri)
        {
            try
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

                List<Story> stories = new List<Story>();

                HtmlNode document = html.DocumentNode;
                var storiesContainer = document.SelectNodes(STORIES_XPATH);
                foreach (var container in storiesContainer)
                {
                    var categoryNode = container.SelectSingleNode("a[1]");
                    var titleNode = container.SelectSingleNode("a[2]");
                    var titleMedalNode = container.SelectSingleNode("a[3]");// if story has medal, title node is next 'a' node
                    var infosNode = container.SelectSingleNode("div[2]");
                    var authorNode = infosNode.SelectSingleNode("div/a");

                    Story story = new Story();
                    story.CategoryUrl = categoryNode.Attributes["href"].Value;
                    story.CategoryName = categoryNode.Attributes["title"].Value.Split('«')[1].Split('»')[0].Trim();
                    story.Title = titleNode.Element("h2")?.InnerText ?? titleMedalNode.Element("h2").InnerText;
                    if (titleNode.Attributes["title"] == null)
                    {
                        story.ChapterName = string.Empty;
                    }
                    else
                    {
                        story.ChapterName = titleNode.Attributes["title"].Value.Contains("«") ? titleNode.Attributes["title"].Value.Split('«')[1].Split('»')[0].Trim() : string.Empty;
                    }
                    story.Url = titleNode.Attributes["href"].Value;

                    story.ReleaseDate = infosNode.Element("time").Attributes["datetime"].Value;

                    Author author = new Author();
                    author.Id = authorNode.Attributes["data-author-id"].Value;
                    author.Url = authorNode.Attributes["href"].Value;
                    author.Name = authorNode.InnerHtml;

                    story.Author = author;

                    stories.Add(story);
                }

                return stories;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.InnerException);
            }
            return null;
        }
    }
}
