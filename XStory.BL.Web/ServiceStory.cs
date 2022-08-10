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

        public const string STORY_HEADER_XPATH = "/html/body/div[1]/main/div[1]/section[1]/div";
        public const string STORY_CONTENT_XPATH = "/html/body/div[1]/main/div[1]/section[2]/div[1]";
        public const string STORY_FOOTER_XPATH = "/html/body/div[1]/main/div[1]/section[3]";

        public ServiceStory()
        {
            _repositoryWeb = new RepositoryWeb();
        }

        public async Task<Story> GetStory(string path)
        {
            string url = string.Concat(_repositoryWeb.GetHttpClient().BaseAddress, path);
            Story story = new Story();
            try
            {
                // Whole page
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await _repositoryWeb.GetHtmlPage(url));

                HtmlNode document = html.DocumentNode;

                // head
                var storyHeaderContainer = document.SelectNodes(STORY_HEADER_XPATH);
                string storyTitle = storyHeaderContainer.FindFirst("h1")?.InnerHtml;

                story.Title = storyTitle;

                var storyContentContainer = document.SelectNodes(STORY_CONTENT_XPATH).Nodes();
                string storyContent = string.Empty;
                foreach (var element in storyContentContainer)
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

                // footer
                var storyFooterContainer = document.SelectNodes(STORY_FOOTER_XPATH);

            }
            catch (Exception e)
            {
                story = null;
            }
            return story;
        }

        public Task<List<Story>> GetStoriesByCategory(int page, string sortCriterion)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Story>> GetStoriesMainPage(int page = 0, string sortCriterion = "")
        {
            try
            {
                string url = string.Concat(_repositoryWeb.GetHttpClient().BaseAddress, "histoires-erotiques", (page > 1 ? ",,," + page : ""), sortCriterion, ".html");
                return await GetStoriesBase(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        private async Task<List<Story>> GetStoriesBase(string url)
        {
            try
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await _repositoryWeb.GetHtmlPage(url));

                List<Story> stories = new List<Story>();

                HtmlNode document = html.DocumentNode;
                var storiesContainer = document.SelectNodes(STORIES_XPATH);
                foreach (var container in storiesContainer)
                {
                    var categoryNode = container.SelectSingleNode("a[1]");
                    var titleNode = container.SelectSingleNode("a[2]");
                    var infosNode = container.SelectSingleNode("div[2]");
                    var authorNode = infosNode.SelectSingleNode("div/a");

                    Story story = new Story();
                    story.CategoryUrl = categoryNode.Attributes["href"].Value;
                    story.CategoryName = categoryNode.Attributes["title"].Value.Split('«')[1].Split('»')[0].Trim();
                    story.Title = titleNode.Element("h2").InnerHtml;
                    story.ChapterName = titleNode.Attributes["title"].Value.Contains("«") ? titleNode.Attributes["title"].Value.Split('«')[1].Split('»')[0].Trim() : string.Empty;
                    story.Url = titleNode.Attributes["href"].Value;

                    story.ReleaseDate = infosNode.Element("time").Attributes["datetime"].Value;

                    Author author = new Author();
                    author.Id = long.Parse(authorNode.Attributes["data-author-id"].Value);
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
