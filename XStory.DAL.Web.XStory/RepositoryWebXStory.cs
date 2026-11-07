using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XStory.DAL.Web.XStory.Contracts;

namespace XStory.DAL.Web.XStory
{
    public class RepositoryWebXStory : IRepositoryWebXStory
    {
        public const string BASE_URL = @"https://www.xstory-fr.com/";

        private static HttpClient _httpClient;
        public static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient(new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                        {
                            //bypass
                            return true;
                        },
                    }, false)
                    { BaseAddress = new Uri(BASE_URL) };
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:141.0) Gecko/20100101 Firefox/141.0");
                    _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml");
                }
                return _httpClient;
            }
        }

        public HttpClient GetHttpClient()
        {
            return HttpClient;
        }

        public async Task<string> GetHtmlPage(string url)
        {
            string htmlPage = string.Empty;
            try
            {
                Uri requestUri = new Uri(url);
                HttpResponseMessage response = await HttpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    string error = response.StatusCode + " " + response.ReasonPhrase;
                    throw new Exception(error);
                }

                string html = await response.Content.ReadAsStringAsync();
                htmlPage = HttpUtility.HtmlDecode(html);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return htmlPage;
        }
    }
}
