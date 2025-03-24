using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer;
using SimpleIdServer.SelfIdServer.Host.Acceptance.Tests;
using SimpleIdServer.SelfIdServer.Host.Acceptance.Tests.Conf;
using System.Linq;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(BuildProtectedSessionStorage());
builder.AddSidIdentityServer()
    .UseEfStore(e => e.UseInMemoryDatabase("idserver"), e => e.UseInMemoryDatabase("formbuilder"))
    .EnableInMemoryMasstransit()
    .EnableCiba();
var antiforgeryService = builder.Services.First(s => s.ServiceType == typeof(IAntiforgery));
var memoryDistribution = builder.Services.First(s => s.ServiceType == typeof(IDistributedCache));
builder.Services.AddDidKey();
builder.Services.Remove(antiforgeryService);
builder.Services.Remove(memoryDistribution);
builder.Services.AddTransient<IAntiforgery, FakeAntiforgery>();
builder.Services.AddSingleton<IDistributedCache>(SingletonDistributedCache.Instance().Get());
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