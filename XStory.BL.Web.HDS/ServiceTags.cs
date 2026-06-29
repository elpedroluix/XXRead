using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.HDS.Contracts;
using XStory.DAL.Web.HDS.Contracts;
using XStory.DTO;
using XStory.Logger;

namespace XStory.BL.Web.HDS
{
    public class ServiceTag : IServiceTag
    {
        private IRepositoryWebHDS _repositoryWeb;

        private BL.Web.HDS.Contracts.IServiceStory _serviceStoryHDS;

        public const string TAGS_XPATH = "/html/body/div/div[2]/div[4]/div/div[3]/div/section[2]/ul";
        public ServiceTag(IRepositoryWebHDS repositoryWeb, BL.Web.HDS.Contracts.IServiceStory serviceStoryHDS)
        {
            _repositoryWeb = repositoryWeb;

            _serviceStoryHDS = serviceStoryHDS;
        }

        public async Task<List<Tag>> GetTags()
        {
            try
            {
                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, "/sexe/histoires-par-date.php");

                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

                List<Tag> tags = new List<DTO.Tag>();

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
            catch (Exception ex)
            {
                ServiceLog.Error(ex);
                return null;
            }
        }
    }
}
