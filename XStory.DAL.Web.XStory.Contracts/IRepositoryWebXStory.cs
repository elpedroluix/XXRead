using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace XStory.DAL.Web.XStory.Contracts
{
    public interface IRepositoryWebXStory
    {
        Task<string> GetHtmlPage(string url);
        HttpClient GetHttpClient();
    }
}
