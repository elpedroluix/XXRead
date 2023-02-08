using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using XStory.BL.Web.HDS.Contracts;
using XStory.DAL.Web;
using XStory.DAL.Web.Contracts;
using XStory.DTO;

namespace XStory.BL.Web.HDS
{
    public class ServiceStory : BL.Web.HDS.Contracts.IServiceStory
    {
        private IRepositoryWeb _repositoryWeb;

        public const string STORIES_XPATH = "/html/body/div/div[2]/div[3]/div/div[2]/div/div[3]/div";

        public const string STORY_CONTAINER_XPATH = "/html/body/div/div[2]/div[3]/div/div[1]";
        public const string STORY_CONTENT_XPATH = "/html/body/div/div[2]/div[3]/div/div[1]/div[5]/div[2]";

        public ServiceStory()
        {
            _repositoryWeb = new RepositoryWebHDS();
        }

        public async Task<List<Story>> GetStoriesMainPage(int page)
        {
            try
            {
                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, string.Concat("sexe/histoires-par-date.php", (page > 1 ? "?p=" + page : "")));
                return await GetStoriesBase(uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<Story>> GetStoriesByCategory(int page)
        {
            throw new NotImplementedException();
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
            var titleNode = storyContainer.SelectSingleNode("h2");
            if (titleNode != null && titleNode.Attributes["class"]?.Value == "title")
            {
                string title = titleNode.InnerText;
                story.Title = title;
            }

            // AUTHOR
            if (story.Author == null)
            {
                var authorNode = storyContainer.SelectSingleNode("div[2]/div[2]/div[1]/a");
                if (authorNode != null && authorNode.Attributes["href"] != null)
                {
                    string authorUrl = authorNode.Attributes["href"].Value;
                    string authorName = authorUrl.Split('=')[1];
                    Author author = new Author()
                    {
                        Name = authorName,
                        Url = authorUrl,
                    };

                    story.Author = author;
                }
            }

            var publishNode = storyContainer.SelectSingleNode("div[3]/div[1]");
            if (publishNode != null && publishNode.Attributes["class"]?.Value == "published_at")
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
                    //07-02-2023 dans la catégorie Entre-nous, hommes et femmes
                    string category = publishInfo.Replace(" dans la catégorie ", "");
                    category = string.Concat(category.Skip(10));

                    story.CategoryName = category;

                    story.CategoryUrl = Helpers.StaticUtils.StoryCategoryUrlDictionary[category];
                }
            }

            int viewsCountDivIndex = 2;

            // TAGS
            // /html/body/div/div[2]/div[3]/div/div[1]/div[3]/div[2]
            var tagsNode = storyContainer.SelectSingleNode("div[3]/div[2]");
            if (tagsNode != null && tagsNode.Attributes["class"]?.Value == "tags")
            {
                // TODO tags

                viewsCountDivIndex++;
            }

            // VIEWS
            var viewsNode = storyContainer.SelectSingleNode("div[3]/div[" + viewsCountDivIndex + "]/b");
            if (viewsNode != null)
            {
                string viewsCount = viewsNode.InnerText;
                story.ViewsNumber = long.Parse(string.Concat(viewsCount.Split(' ')));
            }
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

                List<Story> stories = new List<Story>();

                HtmlNode document = html.DocumentNode;
                var storiesContainer = document.SelectNodes(STORIES_XPATH).Where(node => node.Attributes["class"].Value == "story abstract");
                foreach (var container in storiesContainer)
                {
                    var publishedNode = container.SelectSingleNode("div[1]");
                    var categoryNode = container.SelectSingleNode("div[2]");
                    var authorNode = container.SelectSingleNode("div[3]/a");
                    var titleNode = container.SelectSingleNode("div[4]/a");
                    var textNode = container.SelectSingleNode("div[5]");

                    Story story = new Story();

                    // RELEASE DATE

                    // Histoire erotique publiée sur Histoires De Sexe le 07-02-2023 à 09 heures
                    string releaseDate = publishedNode.InnerText.Replace(" Histoire erotique publiée sur Histoires De Sexe le ", "");

                    //07-02-2023 à 09 heures
                    releaseDate = releaseDate.Replace(" à ", " ");

                    //07-02-2023 09:00:00
                    releaseDate = releaseDate.Replace(" heures", ":00:00");

                    story.ReleaseDate = releaseDate;

                    // TITLE
                    story.Title = titleNode.InnerText;

                    // URL
                    story.Url = titleNode.Attributes["href"].Value;

                    // CATEGORY
                    string category = categoryNode?.InnerText?.Split(':')[1]?.Trim();

                    if (!string.IsNullOrEmpty(category))
                    {
                        story.CategoryName = category;
                        story.CategoryUrl = Helpers.StaticUtils.StoryCategoryUrlDictionary[category];
                    }

                    // AUTHOR
                    Author author = new Author();
                    author.Url = authorNode.Attributes["href"].Value;
                    author.Name = authorNode.InnerText;
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
