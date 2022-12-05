using SimpleIdServer.WsFederation.Controllers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);
var certificate = new X509Certificate2(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "localhost.pfx"), "password");
builder.Services.AddControllers().AddApplicationPart(typeof(WsFederationController).Assembly);
builder.Services.AddWsFederation(o =>
{
    o.SigningCertificate = new (certificate);
});
var app = builder.Build();
app.MapControllers();
app.Run();