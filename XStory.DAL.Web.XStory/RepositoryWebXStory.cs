﻿using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using XStory.DAL.Web.XStory.Contracts;

namespace XStory.DAL.Web
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
                string responseContent = await response.Content.ReadAsStringAsync();
                htmlPage = HttpUtility.HtmlDecode(responseContent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return htmlPage;
        }
    }
}
