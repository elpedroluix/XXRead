using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.BL.Web.HDS.Contracts;
using XStory.DAL.Web;
using XStory.DAL.Web.HDS;
using XStory.DAL.Web.HDS.Contracts;
using XStory.DTO;
using XStory.Logger;

namespace XStory.BL.Web.HDS
{
    public class ServiceCategory : IServiceCategory
    {
        private IRepositoryWebHDS _repositoryWeb;

        public const string CATEGORIES_XPATH = "/html/body/div/div[2]/div[4]/div/div[1]/div/section[3]/ul";

        public ServiceCategory()
        {
            _repositoryWeb = new RepositoryWebHDS();
        }

        public async Task<List<Category>> GetCategories()
        {
            try
            {
                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, "/sexe/histoires-par-date.php");

                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

                List<Category> categories = new List<DTO.Category>();

                HtmlNode document = html.DocumentNode;
                var categoriesContainer = document.SelectNodes(CATEGORIES_XPATH).Descendants("li");
                foreach (var categoryNode in categoriesContainer)
                {
                    string title = categoryNode.SelectSingleNode("a").InnerText;
                    string url = categoryNode.SelectSingleNode("a").Attributes["href"].Value;
                    bool isEnabled = true;


                    Category category = new Category()
                    {
                        Title = title,
                        Source = "HDS",
                        Url = url,
                        IsEnabled = isEnabled
                    };
                    categories.Add(category);
                }
                return categories;
            }
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                return null;
            }
        }
    }
}
