﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XStory.BL.Web.Contracts;
using XStory.DAL.Web;
using XStory.DAL.Web.Contracts;
using XStory.DTO;

namespace XStory.BL.Web
{
    public class ServiceCategory : IServiceCategory
    {
        private IRepositoryWeb _repositoryWeb;

        public const string CATEGORIES_XPATH = "/html/body/div[1]/main/section[1]/div[2]";

        public ServiceCategory()
        {
            _repositoryWeb = new RepositoryWeb();
        }

        public async Task<List<Category>> GetCategories()
        {
            try
            {
                Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, "/histoires-erotiques.html");

                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

                List<Category> categories = new List<DTO.Category>();

                HtmlNode document = html.DocumentNode;
                var categoriesContainer = document.SelectNodes(CATEGORIES_XPATH).Descendants("a");
                foreach (var categoryNode in categoriesContainer)
                {
                    string title = categoryNode.Attributes["data-title"].Value;
                    string url = categoryNode.Attributes["href"].Value;
                    bool isEnabled = true;

                    if (url.Contains("inceste") || url.Contains("zoophilie"))
                    {
                        isEnabled = false;
                    }

                    Category category = new Category()
                    {
                        Title = title,
                        Url = url,
                        IsEnabled = isEnabled
                    };
                    categories.Add(category);
                }
                return categories;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
