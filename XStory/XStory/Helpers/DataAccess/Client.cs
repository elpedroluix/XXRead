using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace XStory.Helpers.DataAccess
{
    public class Client
    {
        private static HttpClient _httpClient;

        public static HttpClient GetInstance()
        {
            if(_httpClient == null){
                _httpClient = new HttpClient() { BaseAddress = new Uri(@"https://www.xstory-fr.com/") };
            }
            return _httpClient;
        }
    }
}