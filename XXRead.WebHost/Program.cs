using XXRead.Core;
using XXRead.Core.Session;
using XXRead.Storage;
using XXRead.UI;
using XXRead.UI.Platform;
using XXRead.Web;
using XXRead.WebHost.Components;
using XXRead.WebHost.Platform;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddXXReadCore().AddXXReadWeb().AddXXReadStorage();
builder.Services.AddXXReadUI();

// One JSON file for both settings interfaces
builder.Services.AddSingleton<ServiceSettingsJson>();
builder.Services.AddSingleton<IServiceSettings>(sp => sp.GetRequiredService<ServiceSettingsJson>());
builder.Services.AddSingleton<IServiceAppSettings>(sp => sp.GetRequiredService<ServiceSettingsJson>());

// Scoped : uses IJSRuntime
builder.Services.AddScoped<IServicePlatform, ServicePlatformWeb>();

var app = builder.Build();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddAdditionalAssemblies(typeof(Routes).Assembly);

app.Run();
