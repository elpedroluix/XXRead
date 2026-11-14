using Bunit;
using Microsoft.Extensions.DependencyInjection;
using XXRead.Core.Services;
using XXRead.Core.Session;
using XXRead.UI;
using XXRead.UI.Platform;
using XXRead.UI.Services;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// bUnit context with the real UI services (AddXXReadUI, MudBlazor) and fakes for Core and the host.
	/// </summary>
	public class BunitContextUI : BunitContext
	{
		public FakeServiceStory ServiceStory { get; } = new FakeServiceStory();

		public FakeServiceCategory ServiceCategory { get; } = new FakeServiceCategory();

		public FakeServiceAuthor ServiceAuthor { get; } = new FakeServiceAuthor();

		public FakeAppSettings AppSettings { get; } = new FakeAppSettings();

		public FakeServiceSettings ServiceSettings { get; } = new FakeServiceSettings();

		public FakeServiceNavigation ServiceNavigation => (FakeServiceNavigation)Services.GetRequiredService<IServiceNavigation>();

		public BunitContextUI()
		{
			// MudBlazor and the pull-to-refresh call JS : unplanned calls return default values
			JSInterop.Mode = JSRuntimeMode.Loose;

			Services.AddXXReadUI();
			// Replaces the real one : history.back() does nothing in bUnit
			Services.AddScoped<IServiceNavigation, FakeServiceNavigation>();
			Services.AddSingleton<IServiceStory>(ServiceStory);
			Services.AddSingleton<IServiceCategory>(ServiceCategory);
			Services.AddSingleton<IServiceAuthor>(ServiceAuthor);
			Services.AddSingleton<IServiceAppSettings>(AppSettings);
			Services.AddSingleton<IServiceSettings>(ServiceSettings);
			Services.AddSingleton<IServicePlatform>(new FakeServicePlatform());
		}
	}
}
