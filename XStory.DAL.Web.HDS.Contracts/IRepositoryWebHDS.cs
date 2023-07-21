using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace XStory.DAL.Web.HDS.Contracts
{
    public interface IRepositoryWebHDS
    {
        Task<string> GetHtmlPage(string url);
        HttpClient GetHttpClient();
    }
}
