using System.Collections;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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
		/// The old RepositoryWeb* create their own HttpClient in a private static field, with no way to inject it:
		/// this field is replaced by a client using the given handler (same base address and headers).
		/// Returns the original client, to restore in TestCleanup.
		/// </summary>
		public static HttpClient SwapHttpClient(Type repositoryType, HttpClient currentHttpClient, HttpMessageHandler handler)
		{
			HttpClient httpClient = new HttpClient(handler, false) { BaseAddress = currentHttpClient.BaseAddress };
			foreach (var header in currentHttpClient.DefaultRequestHeaders)
			{
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
			}

			SetHttpClientField(repositoryType, httpClient);
			return currentHttpClient;
		}

		public static void RestoreHttpClient(Type repositoryType, HttpClient originalHttpClient)
		{
			SetHttpClientField(repositoryType, originalHttpClient);
		}

		private static void SetHttpClientField(Type repositoryType, HttpClient httpClient)
		{
			FieldInfo field = repositoryType.GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Static);
			if (field == null)
			{
				throw new MissingFieldException(repositoryType.FullName, "_httpClient");
			}

			field.SetValue(null, httpClient);
		}

		/// <summary>
		/// Compares the result to Fixtures/[source]/expected/[caseName].json.
		/// On difference, the result is written to Fixtures/[source]/actual/[caseName].json to compare both files.
		/// </summary>
		public static void Check(string source, string caseName, object result, HttpHandlerFixtures handler = null)
		{
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

		private static bool IsEmpty(object result)
		{
			return result == null || (result is ICollection collection && collection.Count == 0);
		}
	}
}
