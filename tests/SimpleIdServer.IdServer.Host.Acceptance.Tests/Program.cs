using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OAuth.Host.Acceptance.Tests;
using System;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSIDIdentityServer()
    .UseInMemoryStore(o =>
    {
        o.AddInMemoryScopes(IdServerConfiguration.Scopes);
        o.AddInMemoryClients(IdServerConfiguration.Clients);
        o.AddInMemoryUsers(IdServerConfiguration.Users);
        o.AddInMemoryApiResources(IdServerConfiguration.ApiResources);
    })
    .AddBackChannelAuthentication()
    .SetSigningKeys(
        new SigningCredentials(BuildRsaSecurityKey("keyid"), SecurityAlgorithms.RsaSha256),
        new SigningCredentials(BuildRsaSecurityKey("keyid2"), SecurityAlgorithms.RsaSha384),
        new SigningCredentials(BuildRsaSecurityKey("keyid3"), SecurityAlgorithms.RsaSha512),
        new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP256), SecurityAlgorithms.EcdsaSha256),
        new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP384), SecurityAlgorithms.EcdsaSha384),
        new SigningCredentials(BuildECDSaSecurityKey(ECCurve.NamedCurves.nistP521), SecurityAlgorithms.EcdsaSha512),
        new SigningCredentials(BuildSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256),
        new SigningCredentials(BuildSymmetricSecurityKey(), SecurityAlgorithms.HmacSha384),
        new SigningCredentials(BuildSymmetricSecurityKey(), SecurityAlgorithms.HmacSha512)
    )
    .SetEncryptedKeys(
        new EncryptingCredentials(BuildRsaSecurityKey("keyid4"), SecurityAlgorithms.RsaPKCS1, SecurityAlgorithms.Aes128CbcHmacSha256)
    )
    .AddAuthentication(o =>
    {
        o.AddMutualAuthentication(m =>
        {
            m.AllowedCertificateTypes = CertificateTypes.All;
            m.RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
        });
    });

var app = builder.Build().UseSID();
app.Run();

static RsaSecurityKey BuildRsaSecurityKey(string keyid) => new RsaSecurityKey(new RSACryptoServiceProvider(2048))
{
    KeyId = keyid
};

static ECDsaSecurityKey BuildECDSaSecurityKey(ECCurve curve) => new ECDsaSecurityKey(ECDsa.Create(curve))
{
    KeyId = Guid.NewGuid().ToString()
};

static SecurityKey BuildSymmetricSecurityKey() => new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(Guid.NewGuid().ToString()));

public partial class Program { }