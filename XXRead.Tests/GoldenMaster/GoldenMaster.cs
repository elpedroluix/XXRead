using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using XXRead.Core.Models;
using XXRead.Core.Sources;
using XXRead.Web;
using XXRead.Web.HDS;
using XXRead.Web.XStory;

namespace XXRead.Tests.GoldenMaster
{
	/// <summary>
	/// Golden master: the scraping code runs on saved HTML pages (Fixtures/[Source]/*.html)
	/// and its result must match the reference JSON (Fixtures/[Source]/expected/*.json),
	/// produced by the code before the refactoring.
	/// The Fixtures folder is local only (.gitignore): without it, the tests are Inconclusive.
	/// </summary>
	public static class GoldenMaster
	{
		/// <summary>
		/// XXREAD_RECORD_FIXTURES=1 : downloads the missing HTML pages and writes the missing references.
		/// Existing files are never overwritten (delete them to record again).
		/// </summary>
		public const string RECORD_ENV_VARIABLE = "XXREAD_RECORD_FIXTURES";

		public static bool IsRecordMode => Environment.GetEnvironmentVariable(RECORD_ENV_VARIABLE) == "1";

		/// <summary>
		/// Properties removed from the code since the references were recorded : ignored in the expected JSON.
		/// The references must never be recorded again (they are the output of the code before the refactoring).
		/// </summary>
		private static readonly string[] _removedProperties = new[]
		{
			// Category.IsEnabled : remains of the removed categories hiding (step 4)
			"IsEnabled",
			// Story.Reviews : List<Post>, never filled, removed with the unused DTOs (step 6)
			"Reviews",
		};

		private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
		{
			WriteIndented = true,
			// XStory: Author.Stories[i].Author is the same author
			ReferenceHandler = ReferenceHandler.IgnoreCycles,
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		};

		public static string GetFixturesDirectory(string source)
		{
			// From bin/Debug/net10.0 up to the project folder
			DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
			while (directory != null && !File.Exists(Path.Combine(directory.FullName, "XXRead.Tests.csproj")))
			{
				directory = directory.Parent;
			}

			if (directory == null)
			{
				throw new DirectoryNotFoundException("XXRead.Tests.csproj not found from " + AppContext.BaseDirectory);
			}

			return Path.Combine(directory.FullName, "Fixtures", source);
		}

		/// <summary>
		/// The sources as registered in the app (<see cref="DependencyInjection.AddXXReadWeb"/> : base address, headers),
		/// but the HTTP requests are answered by <paramref name="handler"/> instead of the websites.
		/// </summary>
		public static ServiceProvider BuildServiceProvider(HttpMessageHandler handler)
		{
			ServiceCollection services = new ServiceCollection();
			services.AddXXReadWeb();

			// Configured after AddXXReadWeb : replaces its primary handler
			services.AddHttpClient(SourceHDS.HTTP_CLIENT_NAME).ConfigurePrimaryHttpMessageHandler(() => handler);
			services.AddHttpClient(SourceXStory.HTTP_CLIENT_NAME).ConfigurePrimaryHttpMessageHandler(() => handler);

			return services.BuildServiceProvider();
		}

		public static ISource GetSource(IServiceProvider serviceProvider, DataSources dataSource)
		{
			return serviceProvider.GetServices<ISource>().Single(s => s.Id == dataSource);
		}

		/// <summary>
		/// Runs <paramref name="scraping"/> and compares its result to Fixtures/[source]/expected/[caseName].json.
		/// On difference, the result is written to Fixtures/[source]/actual/[caseName].json to compare both files.
		/// </summary>
		public static async Task Check<T>(string source, string caseName, Func<Task<T>> scraping, HttpHandlerFixtures handler = null)
		{
			T result;
			try
			{
				result = await scraping();
			}
			catch (Exception) when (handler != null && handler.MissingUrls.Count > 0)
			{
				// The sources throw on a missing page (404)
				result = default;
			}

			if (handler != null && handler.MissingUrls.Count > 0)
			{
				Assert.Inconclusive("Missing HTML fixture: " + string.Join(", ", handler.MissingUrls)
					+ ". To record it: " + RECORD_ENV_VARIABLE + "=1 dotnet test");
			}

			string sourceDirectory = GetFixturesDirectory(source);
			string expectedPath = Path.Combine(sourceDirectory, "expected", caseName + ".json");
			string actualPath = Path.Combine(sourceDirectory, "actual", caseName + ".json");
			string actualJson = JsonSerializer.Serialize(result, _jsonOptions);

			if (!File.Exists(expectedPath))
			{
				if (!IsRecordMode)
				{
					Assert.Inconclusive("Missing reference: " + expectedPath);
				}

				// Never record a failed scraping as the reference
				if (IsEmpty(result))
				{
					Assert.Fail("Empty result, reference not written: " + caseName);
				}

				Directory.CreateDirectory(Path.GetDirectoryName(expectedPath));
				File.WriteAllText(expectedPath, actualJson);
				Assert.Inconclusive("Reference written: " + expectedPath);
			}

			// Compared as JSON trees: the order of the properties does not matter
			JsonNode expected = JsonNode.Parse(File.ReadAllText(expectedPath));
			JsonNode actual = JsonNode.Parse(actualJson);
			RemoveProperties(expected, _removedProperties);

			if (JsonNode.DeepEquals(expected, actual))
			{
				if (File.Exists(actualPath))
				{
					File.Delete(actualPath);
				}
				return;
			}

			Directory.CreateDirectory(Path.GetDirectoryName(actualPath));
			File.WriteAllText(actualPath, actualJson);
			Assert.Fail("Result differs from the reference." + Environment.NewLine
				+ "Expected: " + expectedPath + Environment.NewLine
				+ "Actual:   " + actualPath);
		}

		private static void RemoveProperties(JsonNode node, string[] propertyNames)
		{
			if (node is JsonObject jsonObject)
			{
				foreach (string propertyName in propertyNames)
				{
					jsonObject.Remove(propertyName);
				}
				foreach (KeyValuePair<string, JsonNode> property in jsonObject)
				{
					RemoveProperties(property.Value, propertyNames);
				}
			}
			else if (node is JsonArray jsonArray)
			{
				foreach (JsonNode item in jsonArray)
				{
					RemoveProperties(item, propertyNames);
				}
			}
		}

		private static bool IsEmpty(object result)
		{
			return result == null || (result is ICollection collection && collection.Count == 0);
		}
	}
}
