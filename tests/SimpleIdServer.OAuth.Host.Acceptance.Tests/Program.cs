using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OAuth.Host.Acceptance.Tests;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSIDIdentityServer()
    .AddInMemoryScopes(IdServerConfiguration.Scopes)
    .AddInMemoryClients(IdServerConfiguration.Clients)
    .AddInMemoryUsers(IdServerConfiguration.Users)
    .AddSigningKey(BuildSecurityKey())
    .AddMutualAuthentication(callback: o =>
    {
        o.AllowedCertificateTypes = CertificateTypes.All;
    });

var app = builder.Build().UseSID();
app.Run();

static RsaSecurityKey BuildSecurityKey() => new RsaSecurityKey(new RSACryptoServiceProvider(2048))
{
    KeyId = "keyid"
};

public partial class Program { }