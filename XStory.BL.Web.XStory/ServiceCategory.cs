using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XStory.BL.Web.XStory.Contracts;
using XStory.DAL.Web;
using XStory.DAL.Web.XStory;
using XStory.DAL.Web.XStory.Contracts;
using XStory.DTO;
using XStory.Logger;

namespace XStory.BL.Web.XStory
{
	public class ServiceCategory : IServiceCategory
	{
		private IRepositoryWebXStory _repositoryWeb;

		public const string CATEGORIES_XPATH = "//nav[@aria-label='Orientations']//a[contains(@class,'discover-stories__filter--category')]";

		public ServiceCategory()
		{
			_repositoryWeb = new RepositoryWebXStory();
		}

		public async Task<List<Category>> GetCategories()
		{
			try
			{
				Uri uri = new Uri(_repositoryWeb.GetHttpClient().BaseAddress, "/histoires-erotiques");

				HtmlDocument html = new HtmlDocument();
				html.LoadHtml(await _repositoryWeb.GetHtmlPage(uri.ToString()));

				List<Category> categories = new List<DTO.Category>();

				HtmlNode document = html.DocumentNode;
				var categoryNodes = document.SelectNodes(CATEGORIES_XPATH);

				if (categoryNodes == null) return categories;

				foreach (var categoryNode in categoryNodes)
				{
					string title = categoryNode.InnerText.Trim();
					string url = categoryNode.Attributes["href"]?.Value ?? string.Empty;

					// Skip "Tout" (the unfiltered all-categories link)
					if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url) || title == "Tout")
						continue;

					Category category = new Category()
					{
						Title = title,
						Source = "XStory",
						Url = url,
						IsEnabled = true
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
