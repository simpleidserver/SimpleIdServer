using SimpleIdServer.IdServer.Website.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
.AddEnvironmentVariables();
var idserverAdminConfiguration = builder.Configuration.Get<IdentityServerAdminConfiguration>();
var adminBuilder = builder.Services.AddIdserverAdmin(idserverAdminConfiguration.IdserverBaseUrl, o =>
{
    o.ScimUrl = idserverAdminConfiguration.ScimBaseUrl;
});
if(!string.IsNullOrWhiteSpace(idserverAdminConfiguration.DataProtectionPath))
{
    adminBuilder.PersistDataprotection(idserverAdminConfiguration.DataProtectionPath);
}

if(idserverAdminConfiguration.IsRealmEnabled)
{
    adminBuilder.EnableRealm();
}

if (idserverAdminConfiguration.ForceHttps)
{
    adminBuilder.ForceHttps();
}

adminBuilder.UpdateOpenid(idserverAdminConfiguration.ClientId, idserverAdminConfiguration.ClientSecret, idserverAdminConfiguration.Scopes, idserverAdminConfiguration.IgnoreCertificateError);
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseIdserverAdmin();
app.Run();