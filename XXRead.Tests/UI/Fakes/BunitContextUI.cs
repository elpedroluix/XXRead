using Bunit;
using Microsoft.Extensions.DependencyInjection;
using XXRead.Core.Services;
using XXRead.Core.Session;
using XXRead.UI;
using XXRead.UI.Platform;

namespace XXRead.Tests.UI.Fakes
{
	/// <summary>
	/// bUnit context with the real UI services (AddXXReadUI, MudBlazor) and fakes for Core and the host.
	/// </summary>
	public class BunitContextUI : BunitContext
	{
		public FakeServiceStory ServiceStory { get; } = new FakeServiceStory();

		public FakeServiceCategory ServiceCategory { get; } = new FakeServiceCategory();

		public FakeAppSettings AppSettings { get; } = new FakeAppSettings();

		public BunitContextUI()
		{
			// MudBlazor and the pull-to-refresh call JS : unplanned calls return default values
			JSInterop.Mode = JSRuntimeMode.Loose;

			Services.AddXXReadUI();
			Services.AddSingleton<IServiceStory>(ServiceStory);
			Services.AddSingleton<IServiceCategory>(ServiceCategory);
			Services.AddSingleton<IServiceAppSettings>(AppSettings);
			Services.AddSingleton<IServiceSettings>(new FakeServiceSettings());
			Services.AddSingleton<IServicePlatform>(new FakeServicePlatform());
		}
	}
}
