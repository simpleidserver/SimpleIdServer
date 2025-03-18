using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Key;
using SimpleIdServer.IdServer;
using SimpleIdServer.SelfIdServer.Host.Acceptance.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FormBuilder;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.JSInterop;
using Moq;
using Hangfire;
var builder = WebApplication.CreateBuilder(args);
var p = GetKey(512);
var s = new SymmetricSecurityKey(p)
{
    KeyId = Guid.NewGuid().ToString()
};

builder.Services.AddSingleton(BuildProtectedSessionStorage());
builder.Services.AddHangfire(o => {
    o.UseRecommendedSerializerSettings();
    o.UseIgnoredAssemblyVersionTypeResolver();
    o.UseInMemoryStorage();
});
builder.Services.AddSidIdentityServer()
    .UseInMemoryEFStore(o =>
    {
        o.AddInMemoryRealms(IdServerConfiguration.Realms);
        o.AddInMemoryKeys(SimpleIdServer.IdServer.Config.DefaultRealms.Master, new List<SigningCredentials>
        {
            new SigningCredentials(BuildRsaSecurityKey("keyid"), SecurityAlgorithms.RsaSha256),
            new SigningCredentials(BuildRsaSecurityKey("keyid2"), SecurityAlgorithms.RsaSha384),
            new SigningCredentials(BuildRsaSecurityKey("keyid3"), SecurityAlgorithms.RsaSha512),
            new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP256), SecurityAlgorithms.EcdsaSha256),
            new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP384), SecurityAlgorithms.EcdsaSha384),
            new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP521), SecurityAlgorithms.EcdsaSha512),
            new SigningCredentials(BuildSymmetricSecurityKey(256), SecurityAlgorithms.HmacSha256),
            new SigningCredentials(BuildSymmetricSecurityKey(384), SecurityAlgorithms.HmacSha384),
            new SigningCredentials(BuildSymmetricSecurityKey(512), SecurityAlgorithms.HmacSha512)
        }, new List<EncryptingCredentials>
        {
            new EncryptingCredentials(BuildRsaSecurityKey("keyid4"), SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256)
        });
    })
    .EnableCiba();
builder.Services.AddFormBuilder().UseEf();
var antiforgeryService = builder.Services.First(s => s.ServiceType == typeof(IAntiforgery));
var memoryDistribution = builder.Services.First(s => s.ServiceType == typeof(IDistributedCache));
builder.Services.AddDidKey();
builder.Services.Remove(antiforgeryService);
builder.Services.Remove(memoryDistribution);
builder.Services.AddTransient<IAntiforgery, FakeAntiforgery>();
builder.Services.AddSingleton<IDistributedCache>(SingletonDistributedCache.Instance().Get());
var app = builder.Build()
    .UseSid();
app.Run();

static RsaSecurityKey BuildRsaSecurityKey(string keyid) => new RsaSecurityKey(RSA.Create())
{
    KeyId = keyid
};

static ECDsaSecurityKey BuildECDSaSecurityKey(ECCurve curve) => new ECDsaSecurityKey(ECDsa.Create(curve))
{
    KeyId = Guid.NewGuid().ToString()
};

static SecurityKey BuildSymmetricSecurityKey(int keySize) => new SymmetricSecurityKey(GetKey(keySize))
{
    KeyId = Guid.NewGuid().ToString()
};

static byte[] GetKey(int keySize)
{
    var length = keySize / 8;
    var str = "abcdefghijklmnopqrstuvwxyz";
    var rnd = new Random();
    return Enumerable.Repeat(0, length).Select(_ => rnd.Next(0, str.Length)).Select(_ => Convert.ToByte(str[_])).ToArray();
}

static ProtectedSessionStorage BuildProtectedSessionStorage()
{
    var mockJsInterop = new Mock<IJSRuntime>();
    var mockDataProtectionProvider = new Mock<IDataProtectionProvider>();
    var obj = new ProtectedSessionStorage(mockJsInterop.Object, mockDataProtectionProvider.Object);
    return obj;
}

public partial class Program { }