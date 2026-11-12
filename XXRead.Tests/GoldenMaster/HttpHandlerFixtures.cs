using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace XXRead.Tests.GoldenMaster
{
	/// <summary>
	/// Serves the HTML pages saved in Fixtures/[Source]/ instead of calling the website.
	/// In record mode, a missing page is downloaded once from the real website and saved.
	/// </summary>
	public class HttpHandlerFixtures : DelegatingHandler
	{
		private readonly string _fixturesDirectory;

		/// <summary>
		/// Requested URLs with no saved page: the test result is meaningless (Inconclusive).
		/// </summary>
		public List<string> MissingUrls { get; } = new List<string>();

		public HttpHandlerFixtures(string fixturesDirectory) : base(new HttpClientHandler())
		{
			_fixturesDirectory = fixturesDirectory;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			string filePath = Path.Combine(_fixturesDirectory, GetFileName(request.RequestUri));

			if (!File.Exists(filePath))
			{
				if (!GoldenMaster.IsRecordMode)
				{
					MissingUrls.Add(request.RequestUri.ToString());
					return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
				}

				HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
				if (!response.IsSuccessStatusCode)
				{
					MissingUrls.Add(request.RequestUri + " (" + (int)response.StatusCode + ")");
					return response;
				}

				// Saved as decoded text (UTF-8): the code under test gets exactly the same string as from the website
				Directory.CreateDirectory(_fixturesDirectory);
				await File.WriteAllTextAsync(filePath, await response.Content.ReadAsStringAsync(cancellationToken), cancellationToken);
			}

			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				RequestMessage = request,
				Content = new StringContent(await File.ReadAllTextAsync(filePath, cancellationToken), Encoding.UTF8, "text/html"),
			};
		}

		/// <summary>
		/// File name built from the path and query, unescaped first so that "%28" and "(" give the same file.
		/// </summary>
		public static string GetFileName(Uri uri)
		{
			string pathAndQuery = Uri.UnescapeDataString(uri.PathAndQuery);

			string name = new string(pathAndQuery.TrimStart('/')
				.Select(c => char.IsAsciiLetterOrDigit(c) || c == '.' || c == '-' ? c : '_')
				.ToArray());

			if (name.Length == 0)
			{
				name = "index";
			}
			else if (name.Length > 100)
			{
				string hash = Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(pathAndQuery))).Substring(0, 8);
				name = name.Substring(0, 100) + "_" + hash;
			}

			return name + ".html";
		}
	}
}
