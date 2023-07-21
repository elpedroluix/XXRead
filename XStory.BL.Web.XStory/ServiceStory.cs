using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using XStory.BL.Web.XStory.Contracts;
using XStory.BL.Web.XStory.Helpers;
using XStory.DAL.Web;
using XStory.DAL.Web.XStory.Contracts;
using XStory.DTO;

namespace XStory.BL.Web.XStory
{
    public class ServiceStory : IServiceStory
    {
        private IRepositoryWebXStory _repositoryWeb;

        public const string HTML_BR = "br";
        public const string HTML_CLASS = "class";
        public const string HTML_DATETIME = "datetime";
        public const string HTML_DIV = "div";
        public const string HTML_FA_QUESTION = "fa fa-question";
        public const string HTML_FA_EYE = "fa fa-eye";
        public const string HTML_HREF = "href";
        public const string HTML_TIME = "time";
        public const string HTML_TITLE = "title";

        public const string CHAPTER_TITLE_BEGIN = "Chapitre ";
        public const string UNIQUE = "unique";

        public const string XS_LIRE_HISTOIRE_PARAGRAPHE = "xs-lire-histoire-paragraphe";

        public const string STORIES_XPATH = "/html/body/div[1]/main/section[4]/div[2]/ul/li/div";

        public const string STORY_HEADER_XPATH = "/html/body/div[1]/main/div[1]/section[1]";
        public const string STORY_CONTENT_XPATH = "/html/body/div[1]/main/div[1]/section[2]";
        public const string STORY_FOOTER_XPATH = "/html/body/div[1]/main/div[1]/section[3]";

        public const string STORY_CHAPTERS_XPATH = "/html/body/div[1]/aside/div[1]/section[1]/div/ul/li";


        public ServiceStory()
        {
            _repositoryWeb = new RepositoryWebXStory();
        }

        public async Task<List<Story>> GetStoriesPage(int page = 0, string categoryUrl = "", string sortCriterion = "")
        {
            try
            {
                string basePath = "/histoires-erotiques";
                string categoryPath = string.Empty;
                string pagePath;
                string endPath = ".html";

                if (!string.IsNullOrWhiteSpace(categoryUrl))
                {
                    categoryPath = StaticUtils.CategoryFromUrlDictionary[categoryUrl];
                    pagePath = page == 0 ? "" : "," + page;
                }
                else
                {
                    pagePath = page > 1 ? ",,," + page : "";
                }

                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress,
                    string.Concat(basePath, categoryPath, pagePath, sortCriterion, endPath));
                return await GetStoriesBase(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
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
                HtmlNode storyHeaderContainer = document.SelectSingleNode(STORY_HEADER_XPATH);
                this.InitStoryInfos(story, storyHeaderContainer);

                // HEADER title + chapters
                this.InitStoryTitleChapters(story, storyHeaderContainer);

                // HEADER category
                this.InitStoryCategory(story, storyHeaderContainer);


                // CONTENT
                HtmlNode storyContentContainer = document.SelectSingleNode(STORY_CONTENT_XPATH);
                this.InitStoryContent(story, storyContentContainer);


                // chapters list
                var storyChaptersList = document.SelectNodes(STORY_CHAPTERS_XPATH);

                if (storyChaptersList != null)
                {
                    GetStoryChapters(story, storyChaptersList);
                }

            }
            catch (Exception ex)
            {
                story = null;
                Logger.ServiceLog.Error(ex);
            }
            return story;
        }

        private void InitStoryInfos(Story story, HtmlNode storyHeaderContainer)
        {
            // Begin <li>'s
            int headerLiTagCount = 1;
            const string LI_XPATH = "div/div[1]/ul/li[";

            // AUTHOR
            if (story.Author == null)
            {
                string headerAuthorXPath = LI_XPATH + headerLiTagCount + "]/a";
                Author author = new Author()
                {
                    Id = storyHeaderContainer.SelectSingleNode(headerAuthorXPath).Attributes["data-author-id"].Value,
                    Name = storyHeaderContainer.SelectSingleNode(headerAuthorXPath).InnerText,
                    Url = storyHeaderContainer.SelectSingleNode(headerAuthorXPath).Attributes[HTML_HREF].Value
                };
                story.Author = author;

                headerLiTagCount++;
            }

            // TYPE
            string headerTypeXPath = LI_XPATH + headerLiTagCount + "]";
            var iType = storyHeaderContainer.SelectSingleNode(headerTypeXPath + "/i");
            if (iType.Attributes[HTML_CLASS].Value == HTML_FA_QUESTION)
            {
                string type = storyHeaderContainer.SelectSingleNode(headerTypeXPath + "/text()[2]").InnerText;
                type = type.Replace(@"\n", "").Replace(@"\t", "").Trim();
                story.Type = type;

                headerLiTagCount++;
            }

            // RELEASE DATE
            if (string.IsNullOrWhiteSpace(story.ReleaseDate))
            {
                string headerReleaseDateXPath = LI_XPATH + headerLiTagCount + "]/time";

                story.ReleaseDate = storyHeaderContainer.SelectSingleNode(headerReleaseDateXPath).Attributes[HTML_DATETIME].Value;
                headerLiTagCount++;
            }

            // VIEWS NUMBER
            string headerViewsNumberXPath = LI_XPATH + headerLiTagCount + "]";
            var iViewsNumber = storyHeaderContainer.SelectSingleNode(headerViewsNumberXPath + "/i");
            if (iViewsNumber.Attributes[HTML_CLASS].Value == HTML_FA_EYE)
            {
                story.ViewsNumber = long.Parse(string.Concat(storyHeaderContainer.SelectSingleNode(headerViewsNumberXPath + "/span").InnerText.Split(' ')));
                headerLiTagCount++;
            }

            // LIKES NUMBER
            string headerLikesNumberXPath = LI_XPATH + headerLiTagCount + "]/a/span";
            story.LikesNumber = long.Parse(storyHeaderContainer.SelectSingleNode(headerLikesNumberXPath).InnerText);
            headerLiTagCount++;

            // REVIEWS NUMBER
            string headerReviewsNumberXPath = LI_XPATH + headerLiTagCount + "]/a";
            story.ReviewsNumber = long.Parse(storyHeaderContainer.SelectSingleNode(headerReviewsNumberXPath).InnerText.Split(' ')[0]);

            // End of <li>'s
        }

        private void InitStoryTitleChapters(Story story, HtmlNode storyHeaderContainer)
        {
            int chapterh2TagCount = 1;
            const string TITLE_CHAPTERS_XPATH = "div/div[3]";

            story.Title = storyHeaderContainer.SelectSingleNode(TITLE_CHAPTERS_XPATH + "/h1").InnerText;

            // Chapter number ("Chapitre x")
            string chapterNumberXPath = TITLE_CHAPTERS_XPATH + "/h2[" + chapterh2TagCount + "]";
            string chapterTryNumber = storyHeaderContainer.SelectSingleNode(chapterNumberXPath).InnerText.Split(' ')[1];
            story.ChapterNumber = chapterTryNumber == UNIQUE ? 1 : int.Parse(chapterTryNumber);
            chapterh2TagCount++;

            // Chapter name                
            story.ChapterName = storyHeaderContainer.SelectSingleNode(chapterNumberXPath).InnerText;
            string chapterNameXPath = TITLE_CHAPTERS_XPATH + "/h2[" + chapterh2TagCount + "]";
            if (storyHeaderContainer.SelectSingleNode(chapterNameXPath) != null)
            {
                story.ChapterName +=
                    string.Concat(" : "
                    , storyHeaderContainer.SelectSingleNode(chapterNameXPath)?.InnerText ?? string.Empty);
            }
        }


        private void InitStoryCategory(Story story, HtmlNode storyHeaderContainer)
        {
            const string CATEGORY_XPATH = "div/div[3]/a";
            story.CategoryName = storyHeaderContainer.SelectSingleNode(CATEGORY_XPATH + "/div[2]").InnerText;
            story.CategoryUrl = storyHeaderContainer.SelectSingleNode(CATEGORY_XPATH).Attributes[HTML_HREF].Value;
        }

        /// <summary>
        /// Defines the story content.
        /// </summary>
        /// <param name="story">Story to build</param>
        /// <param name="storyContentContainer"></param>
        private void InitStoryContent(Story story, HtmlNode storyContentContainer)
        {
            var storyContentParagraphsContainer = storyContentContainer.SelectNodes(HTML_DIV).Nodes();

            string storyContent = string.Empty;

            //storyContent += storyContentContainer.InnerHtml;
            foreach (var element in storyContentParagraphsContainer)
            {
                if (element.Name == "p:p")
                {
                    if (element.Attributes[HTML_CLASS].Value == XS_LIRE_HISTOIRE_PARAGRAPHE)
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
                else if (element.Name == HTML_BR)
                {
                    storyContent += Environment.NewLine + Environment.NewLine;
                }
            }
            story.Content = storyContent;
        }

        private void GetStoryChapters(Story story, HtmlNodeCollection storyChaptersList)
        {
            foreach (var storyChapter in storyChaptersList)
            {
                Story chapterStory = new Story();
                chapterStory.Author = story.Author;

                // DATE
                chapterStory.ReleaseDate = storyChapter.SelectSingleNode(HTML_TIME).Attributes[HTML_DATETIME].Value;

                // - CHAPTER CATEGORY
                chapterStory.CategoryName = Helpers.StaticUtils.CategoryNameDictionary[storyChapter.SelectSingleNode("a/i").Attributes[HTML_CLASS].Value.Split(' ')[1]];

                // - CHAPTER NUMBER
                chapterStory.ChapterNumber = int.Parse(storyChapter.SelectSingleNode("a").InnerText.Split(' ')[2]);

                // - CHAPTER NAME
                chapterStory.ChapterName = storyChapter.SelectSingleNode("a").Attributes[HTML_TITLE]?.Value ?? string.Empty;

                // Beautify display chapter title
                if (chapterStory.ChapterNumber > 0)
                {
                    if (!string.IsNullOrWhiteSpace(chapterStory.ChapterName))
                    {
                        chapterStory.Title = string.Concat(CHAPTER_TITLE_BEGIN, chapterStory.ChapterNumber, " : ", chapterStory.ChapterName);
                    }
                    else
                    {
                        chapterStory.Title = string.Concat(CHAPTER_TITLE_BEGIN, chapterStory.ChapterNumber);
                    }
                }

                // - LIKES
                chapterStory.LikesNumber = long.Parse(storyChapter.SelectSingleNode("div/span").InnerText);

                // - URL
                chapterStory.Url = storyChapter.SelectSingleNode("a").Attributes[HTML_HREF]?.Value ?? string.Empty;

                story.ChaptersList.Add(chapterStory);
            }
        }


        /* TO DELETE ?
        public async Task<List<Story>> GetStoriesByCategory(int page = 0, string categoryUrl = "", string sortCriterion = "")
        {
            try
            {
                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, string.Concat("/histoires-erotiques",
                    StaticUtils.CategoryFromUrlDictionary[categoryUrl], page == 0 ? "" : "," + page, sortCriterion, ".html"));
                return await GetStoriesBase(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }*/

        /* TO DELETE ?
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
        }*/

        /* TO DELETE ?
        public async Task<List<Story>> GetFilteredStoriesMainPage(int page = 0, List<string> hiddenCategories = null, string sortCriterion = "")
        {
            try
            {
                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, string.Concat("/histoires-erotiques", (page > 1 ? ",,," + page : ""), sortCriterion, ".html"));
                var stories = await GetStoriesBase(uri);

                stories = this.FilterStories(stories, hiddenCategories);

                return stories;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }*/

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
    }
}
