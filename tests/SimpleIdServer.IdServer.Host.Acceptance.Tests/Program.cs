using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer.Host.Acceptance.Tests;
using SimpleIdServer.IdServer.Host.Acceptance.Tests.Config;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(BuildProtectedSessionStorage());
builder.AddSidIdentityServer(o =>
{
}).UseEfStore(e => e.UseInMemoryDatabase("idserver"), e => e.UseInMemoryDatabase("formbuilder"))
    .AddPwdAuthentication()
    .AddOpenidFederation(o =>
    {
        o.IsFederationEnabled = true;
        o.TokenSignedKid = "keyid";
    })
    .EnableFapiSecurityProfile(callback: cb =>
    {
        cb.AllowedCertificateTypes = CertificateTypes.All;
        cb.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
    })
    .EnableCiba();

var antiforgeryService = builder.Services.First(s => s.ServiceType == typeof(IAntiforgery));
var memoryDistribution = builder.Services.First(s => s.ServiceType == typeof(IDistributedCache));
builder.Services.Remove(antiforgeryService);
builder.Services.Remove(memoryDistribution);
builder.Services.AddTransient<IAntiforgery, FakeAntiforgery>();
builder.Services.AddSingleton<IDistributedCache>(SingletonDistributedCache.Instance().Get());
builder.Services.AddDidKey();
var app = builder.Build();
Dataseeder.Seed(app);
app.UseSid();
app.Run();

static ProtectedSessionStorage BuildProtectedSessionStorage()
{
    var mockJsInterop = new Mock<IJSRuntime>();
    var mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
    var obj = new ProtectedSessionStorage(mockJsInterop.Object, mockDataProtectionProvider.Object);
    return obj;
}

public partial class Program { }