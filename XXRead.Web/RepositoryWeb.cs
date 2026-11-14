using HtmlAgilityPack;
using System.Web;

namespace XXRead.Web
{
	/// <summary>
	/// Downloads the HTML pages of a source (merge of the former RepositoryWebHDS and RepositoryWebXStory,
	/// only the base address and the headers of the HttpClient differ : see <see cref="DependencyInjection"/>).
	/// </summary>
	internal class RepositoryWeb
	{
		private readonly HttpClient _httpClient;

		public RepositoryWeb(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public Uri BaseAddress => _httpClient.BaseAddress;

		/// <summary>
		/// Throws if the response status is not successful.
		/// </summary>
		public async Task<HtmlDocument> GetHtmlDocument(string url)
		{
			Uri requestUri = new Uri(url);
			HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

			if (!response.IsSuccessStatusCode)
			{
				string error = response.StatusCode + " " + response.ReasonPhrase;
				throw new HttpRequestException(error);
			}

			string html = await response.Content.ReadAsStringAsync();

			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(HttpUtility.HtmlDecode(html));

			return htmlDocument;
		}
	}
}
