using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XStory.Models;

namespace XStory.Helpers.DataAccess
{
    public class Service
    {
        public const string CATEGORIES_XPATH = "/html/body/div[1]/main/section[1]/div[2]";
        public const string STORIES_BY_CATEGORIES_XPATH = "/html/body/div[1]/main/section[4]/div[2]/ul/li/div/a";
        public const string STORIES_MAIN_PAGE_XPATH = "/html/body/div[2]/main/section[4]/div[2]/ul";
        public const string STORIES_XPATH = "/html/body/div[1]/main/section[4]/div[2]/ul/li/div/a";

        public const string STORY_HEADER_XPATH = "/html/body/div[1]/main/div[1]/section[1]/div";
        public const string STORY_CONTENT_XPATH = "/html/body/div[1]/main/div[1]/section[2]/div[1]";
        public const string STORY_FOOTER_XPATH = "/html/body/div[1]/main/div[1]/section[3]";

        private async Task<List<Story>> GetStoriesBase(string url)
        {
            try
            {
                Uri requestUri = new Uri(url);
                HttpResponseMessage response = await Client.GetInstance().GetAsync(requestUri);

                List<Story> stories = new List<Story>();

                if (response.IsSuccessStatusCode)
                {
                    HtmlDocument html = new HtmlDocument();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    html.LoadHtml(HttpUtility.HtmlDecode(responseContent));

                    HtmlNode document = html.DocumentNode;
                    var storiesContainer = document.SelectNodes(STORIES_XPATH);
                    foreach (var storyNode in storiesContainer)
                    {
                        if (storyNode.ChildNodes.Count > 0)
                        {
                            string chapter = string.Empty;
                            var chapterBeforeSplit = storyNode.Attributes["title"].Value.Split('"');
                            if(chapterBeforeSplit.Length > 1)
                            {
                                chapter = chapterBeforeSplit[1];
                            }
                            stories.Add(new Story()
                            {
                                // Fucking ugly but works.....
                                Title = storyNode.FirstChild.InnerText,
                                Chapter = chapter,
                                // Category = storyNode.Attributes["title"].Value.Split('"')[1],
                                Url = storyNode.Attributes["href"].Value
                            }); ;
                            // Console.WriteLine(storyNode.FirstChild.InnerText);
                        }

                    }
                    return stories;
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        // A tester avec les XPATH pour chaque prop
        private async Task<List<Story>> GetStoriesBaseWithCategory(string url)
        {
            try
            {
                Uri requestUri = new Uri(url);
                HttpResponseMessage response = await Client.GetInstance().GetAsync(requestUri);

                List<Story> stories = new List<Story>();

                if (response.IsSuccessStatusCode)
                {
                    HtmlDocument html = new HtmlDocument();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    html.LoadHtml(HttpUtility.HtmlDecode(responseContent));

                    HtmlNode document = html.DocumentNode;
                    var storiesContainer = document.SelectNodes(STORIES_XPATH);
                    foreach (var storyNode in storiesContainer)
                    {
                        if (storyNode.ChildNodes.Count > 0)
                        {
                            string chapter = string.Empty;
                            var chapterBeforeSplit = storyNode.Attributes["title"].Value.Split('"');
                            if (chapterBeforeSplit.Length > 1)
                            {
                                chapter = chapterBeforeSplit[1];
                            }
                            stories.Add(new Story()
                            {
                                // Fucking ugly but works.....
                                Title = storyNode.FirstChild.InnerText,
                                Chapter = chapter,
                                // Category = storyNode.Attributes["title"].Value.Split('"')[1],
                                Url = storyNode.Attributes["href"].Value
                            }); ;
                            // Console.WriteLine(storyNode.FirstChild.InnerText);
                        }

                    }
                    return stories;
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public async Task<List<Story>> GetStoriesMainPage(int page = 0, string sortCriterion = "")
        {
            string url = string.Concat(Client.GetInstance().BaseAddress, "histoires-erotiques", (page > 1 ? ",,," + page : ""), sortCriterion, ".html");
            return await GetStoriesBase(url);
        }

        public async Task<List<Category>> GetCategories()
        {
            Uri requestUri = new Uri(Client.GetInstance().BaseAddress + "histoires-erotiques.html");
            HttpResponseMessage response = await Client.GetInstance().GetAsync(requestUri);

            List<Category> categories = new List<Category>();

            if (response.IsSuccessStatusCode)
            {
                HtmlDocument html = new HtmlDocument();
                string responseContent = await response.Content.ReadAsStringAsync();
                html.LoadHtml(HttpUtility.HtmlDecode(responseContent));

                HtmlNode document = html.DocumentNode;
                var categoriesContainer = document.SelectNodes(CATEGORIES_XPATH).Descendants("a");
                foreach (var categoryNode in categoriesContainer)
                {
                    categories.Add(new Category()
                    {
                        Title = categoryNode.Attributes["data-title"].Value,
                        Url = categoryNode.Attributes["href"].Value,
                    });
                }
                return categories;
            }

            return null;
        }

        public async Task<List<Story>> GetStoriesByCategory(int page = 0, string sortCriterion = "")
        {
            string url = string.Concat(Client.GetInstance().BaseAddress, "/histoires-erotiques,divers,9", (page > 1 ? "," + page : ""), sortCriterion, ".html");
            return await GetStoriesBase(url);
        }

        public async Task<Story> GetStory(string url)
        {
            Story story = new Story();
            try
            {

                Uri requestUri = new Uri(Client.GetInstance().BaseAddress + url);
                HttpResponseMessage response = await Client.GetInstance().GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    // Whole page
                    HtmlDocument html = new HtmlDocument();
                    string responseContent = await response.Content.ReadAsStringAsync();
                    html.LoadHtml(HttpUtility.HtmlDecode(responseContent));

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
            }
            catch (Exception e)
            {
                story = null;
            }
            return story;
        }
    }
}