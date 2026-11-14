using HtmlAgilityPack;
using XStory.DTO;

namespace XXRead.Web.XStory
{
	internal class ScraperCategory
	{
		private RepositoryWeb _repositoryWeb;

		public const string CATEGORIES_XPATH = "//nav[@aria-label='Orientations']//a[contains(@class,'discover-stories__filter--category')]";

		public ScraperCategory(RepositoryWeb repositoryWeb)
		{
			_repositoryWeb = repositoryWeb;
		}

		public async Task<List<Category>> GetCategories()
		{
			Uri uri = new Uri(_repositoryWeb.BaseAddress, "/histoires-erotiques");

			HtmlDocument html = await _repositoryWeb.GetHtmlDocument(uri.ToString());

			List<Category> categories = new List<Category>();

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
					Url = url
				};
				categories.Add(category);
			}
			return categories;
		}
	}
}
