using BlazorTenant;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProtectBlazorPWAMultiTenant;
using System.Diagnostics;

IServiceProvider serviceProvider = null;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
var store = new InMemoryTenantStore();
var tenantSection = builder.Configuration.GetSection("Tenants");
foreach(var tenant in tenantSection.GetChildren())
{
    var authSection = tenant.GetSection("Auth");
    var record = new Tenant(tenant.GetValue<string>("Identifier"), new Dictionary<string, string>()
    {
        { "Auth.Authority", authSection.GetValue<string>("Authority") },
        { "Auth.ClientId", authSection.GetValue<string>("ClientId") }
    });
    store.TryAdd(record);
}
builder.Services.AddMultiTenantancy(store);

builder.Services.Configure<RemoteAuthenticationOptions<OidcProviderOptions>>(options =>
{
    var tenantStore = serviceProvider.GetRequiredService<ITenantStore>();
    var nav = serviceProvider.GetRequiredService<NavigationManager>();
    var tenantIdentifier = GetTenantIdentifier(nav);
    var tenant = tenantStore.TryGet(tenantIdentifier);
    System.Console.WriteLine("COUCOU 1 " + tenantIdentifier);
    if (!string.IsNullOrEmpty(tenant?.Identifier))
    {
        options.ProviderOptions.ClientId = tenant.Parameters["Auth.ClientId"];
        options.ProviderOptions.Authority = tenant.Parameters["Auth.Authority"];
        options.ProviderOptions.ResponseType = "code";
        MultiTenantRemoteAuthenticationPaths.AssignPathsOptionsForTenantOrDefault(tenant, nav, options);
    }
});

builder.Services.AddOidcAuthentication(options =>
{
    System.Console.WriteLine("COUCOU 2");
    options.ProviderOptions.ResponseType = "code";
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
var build = builder.Build();
serviceProvider = build.Services;
build.Services.AddServiceProviderToMultiTenantRoutes();
await build.RunAsync();

string GetTenantIdentifier(NavigationManager navigationManager)
{
    var uri = navigationManager.Uri;
    var baseUrl = navigationManager.BaseUri;
    uri = uri.Replace(baseUrl, string.Empty);
    var splitted = uri.Split('/');
    if (splitted.Length == 0) return null;
    return splitted[0];
}