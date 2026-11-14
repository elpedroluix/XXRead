using HtmlAgilityPack;
using XXRead.Core.Models;

namespace XXRead.Web.HDS
{
    internal class ScraperTags
    {
        private RepositoryWeb _repositoryWeb;

        public const string TAGS_XPATH = "/html/body/div/div[2]/div[4]/div/div[3]/div/section[2]/ul";
        public ScraperTags(RepositoryWeb repositoryWeb)
        {
            _repositoryWeb = repositoryWeb;
        }

        public async Task<List<Tag>> GetTags()
        {
            Uri uri = new Uri(_repositoryWeb.BaseAddress, "/sexe/histoires-par-date.php");

            HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

            List<Tag> tags = new List<Tag>();

            HtmlNode document = html.DocumentNode;
            var tagsContainer = document.SelectNodes(TAGS_XPATH).Descendants("a");
            foreach (var tagNode in tagsContainer)
            {
                string title = tagNode.InnerText;
                string url = tagNode.Attributes["href"].Value;

                Tag tag = new Tag()
                {
                    Title = title,
                    Url = url,
                };
                tags.Add(tag);
            }
            return tags;
        }
    }
}
