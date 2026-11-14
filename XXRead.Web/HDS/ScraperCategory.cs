using HtmlAgilityPack;
using XStory.DTO;

namespace XXRead.Web.HDS
{
    internal class ScraperCategory
    {
        private RepositoryWeb _repositoryWeb;

        public const string CATEGORIES_XPATH = "//section[@class='stories-category-section']/ul";

        public ScraperCategory(RepositoryWeb repositoryWeb)
        {
            _repositoryWeb = repositoryWeb;
        }

        public async Task<List<Category>> GetCategories()
        {
            Uri uri = new Uri(_repositoryWeb.BaseAddress, "/sexe/histoires-par-date.php");

            HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

            List<Category> categories = new List<Category>();

            HtmlNode document = html.DocumentNode;
            var categoriesContainer = document.SelectNodes(CATEGORIES_XPATH).Descendants("li");
            foreach (var categoryNode in categoriesContainer)
            {
                string title = categoryNode.SelectSingleNode("a").InnerText.Trim();
                string url = categoryNode.SelectSingleNode("a").Attributes["href"].Value;

                Category category = new Category()
                {
                    Title = title,
                    Source = "HDS",
                    Url = url
                };
                categories.Add(category);
            }
            return categories;
        }
    }
}
