using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace XStory.DAL.Web.Contracts
{
    public interface IRepositoryWeb
    {
        Task<string> GetHtmlPage(string url);
        HttpClient GetHttpClient();
    }
}
