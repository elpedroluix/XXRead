using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using XXRead.Core.Models;
using XXRead.Core.Sources;
using XXRead.Web;
using XXRead.Web.HDS;
using XXRead.Web.XStory;

namespace XXRead.Tests.Web
{
	[TestClass]
	public class DependencyInjectionTests
	{
		[TestMethod]
		public void AddXXReadWeb_ResolvesOneSourcePerDataSource()
		{
			using ServiceProvider provider = new ServiceCollection().AddXXReadWeb().BuildServiceProvider();

			List<DataSources> ids = provider.GetServices<ISource>().Select(s => s.Id).OrderBy(id => id).ToList();

			CollectionAssert.AreEqual(new List<DataSources>() { DataSources.XStory, DataSources.HDS, DataSources.Demo }, ids);
		}

		/// <summary>
		/// Each source has its own HttpClient : base address and headers are not mixed up.
		/// </summary>
		[TestMethod]
		public async Task AddXXReadWeb_HttpClientPerSource()
		{
			HttpHandlerRecorder handler = new HttpHandlerRecorder();

			ServiceCollection services = new ServiceCollection();
			services.AddXXReadWeb();
			services.AddHttpClient(SourceHDS.HTTP_CLIENT_NAME).ConfigurePrimaryHttpMessageHandler(() => handler);
			services.AddHttpClient(SourceXStory.HTTP_CLIENT_NAME).ConfigurePrimaryHttpMessageHandler(() => handler);
			using ServiceProvider provider = services.BuildServiceProvider();

			List<ISource> sources = provider.GetServices<ISource>().ToList();

			try
			{
				await sources.Single(s => s.Id == DataSources.HDS).GetCategories();
			}
			catch (Exception)
			{
				// Empty page : HDS throws (no categories node), only the request matters here
			}
			await sources.Single(s => s.Id == DataSources.XStory).GetCategories();

			Assert.AreEqual(2, handler.Requests.Count);

			HttpRequestMessage requestHDS = handler.Requests[0];
			Assert.AreEqual(SourceHDS.BASE_URL + "sexe/histoires-par-date.php", requestHDS.RequestUri.ToString());
			Assert.IsFalse(requestHDS.Headers.Contains("User-Agent"));

			HttpRequestMessage requestXStory = handler.Requests[1];
			Assert.AreEqual(SourceXStory.BASE_URL + "histoires-erotiques", requestXStory.RequestUri.ToString());
			StringAssert.StartsWith(requestXStory.Headers.UserAgent.ToString(), "Mozilla/5.0");
		}

		/// <summary>
		/// The sources throw on an unsuccessful response (the former repositories returned an empty page).
		/// </summary>
		[TestMethod]
		public async Task Source_KO_HttpError()
		{
			HttpHandlerRecorder handler = new HttpHandlerRecorder() { StatusCode = HttpStatusCode.NotFound };

			ServiceCollection services = new ServiceCollection();
			services.AddXXReadWeb();
			services.AddHttpClient(SourceXStory.HTTP_CLIENT_NAME).ConfigurePrimaryHttpMessageHandler(() => handler);
			using ServiceProvider provider = services.BuildServiceProvider();

			ISource source = provider.GetServices<ISource>().Single(s => s.Id == DataSources.XStory);

			await Assert.ThrowsExceptionAsync<HttpRequestException>(() => source.GetStoriesPage(1));
		}

		private class HttpHandlerRecorder : HttpMessageHandler
		{
			public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

			public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				Requests.Add(request);

				return Task.FromResult(new HttpResponseMessage(StatusCode)
				{
					RequestMessage = request,
					Content = new StringContent("<html><body></body></html>", Encoding.UTF8, "text/html"),
				});
			}
		}
	}
}
