using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
var dataProtectionPath = builder.Configuration["dataProtectionPath"]?.ToString();
var isRealmEnabled = bool.Parse(builder.Configuration["IsRealmEnabled"]);
var forceHttpsStr = builder.Configuration["forceHttps"];
var adminBuilder = builder.Services.AddIdserverAdmin(o =>
{
    o.ScimUrl = builder.Configuration["ScimBaseUrl"];
}, builder.Configuration["IdServerBaseUrl"]);
if(!string.IsNullOrWhiteSpace(dataProtectionPath))
{
    adminBuilder.PersistDataprotection(dataProtectionPath);
}

if(isRealmEnabled)
{
    adminBuilder.EnableRealm();
}

if (!string.IsNullOrWhiteSpace(forceHttpsStr) && bool.TryParse(forceHttpsStr, out bool r))
{
    adminBuilder.ForceHttps();
}

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseIdserverAdmin();
app.Run();