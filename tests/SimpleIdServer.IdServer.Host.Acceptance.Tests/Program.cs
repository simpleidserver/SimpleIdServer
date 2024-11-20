﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Host.Acceptance.Tests;
using SimpleIdServer.OAuth.Host.Acceptance.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SimpleIdServer.Did.Key;

var builder = WebApplication.CreateBuilder(args);
var p = GetKey(512);
var s = new SymmetricSecurityKey(p)
{
    KeyId = Guid.NewGuid().ToString()
};

builder.Services.AddSIDIdentityServer(o =>
{
    o.WalletAuthorizationServer = "http://localhost";
})
    .UseInMemoryEFStore(o =>
    {
        o.AddInMemoryBCAuthorize(IdServerConfiguration.BCAuthorizeLst);
        o.AddInMemoryRealms(IdServerConfiguration.Realms);
        o.AddInMemoryScopes(IdServerConfiguration.Scopes);
        o.AddInMemoryClients(IdServerConfiguration.Clients);
        o.AddInMemoryUsers(IdServerConfiguration.Users);
        o.AddInMemoryApiResources(IdServerConfiguration.ApiResources);
        o.AddInMemoryUMAResources(IdServerConfiguration.UmaResources);
        o.AddInMemoryGroups(IdServerConfiguration.Groups);
        o.AddInMemoryUserSessions(IdServerConfiguration.Sessions);
        o.AddInMemoryDeviceCodes(IdServerConfiguration.DeviceAuthCodes);
        o.AddInMemoryFederatonEntities(IdServerConfiguration.FederationEntities);
        o.AddInMemoryKeys(SimpleIdServer.IdServer.Constants.StandardRealms.Master, new List<SigningCredentials>
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
    .UseInMemoryMassTransit()
    .AddConsoleNotification()
    .AddPwdAuthentication()
    .AddBackChannelAuthentication()
    .AddOpenidFederation(o =>
    {
        o.IsFederationEnabled = true;
        o.TokenSignedKid = "keyid";
    })
    .AddAuthentication(o =>
    {
        o.AddMutualAuthentication(m =>
        {
            m.AllowedCertificateTypes = CertificateTypes.All;
            m.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
        });
    });
var antiforgeryService = builder.Services.First(s => s.ServiceType == typeof(IAntiforgery));
var memoryDistribution = builder.Services.First(s => s.ServiceType == typeof(IDistributedCache));
builder.Services.Remove(antiforgeryService);
builder.Services.Remove(memoryDistribution);
builder.Services.AddTransient<IAntiforgery, FakeAntiforgery>();
builder.Services.AddSingleton<IDistributedCache>(SingletonDistributedCache.Instance().Get());
builder.Services.AddDidKey();
var app = builder.Build()
    .UseOpenidFederation()
    .UseSID();
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

public partial class Program { }