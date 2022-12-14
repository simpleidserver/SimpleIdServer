using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.OAuth.Host.Acceptance.Tests;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSIDIdentityServer()
    .AddInMemoryScopes(IdServerConfiguration.Scopes)
    .AddSigningKey(BuildSecurityKey());

var app = builder.Build();
app.MapControllers();
app.Run();

static RsaSecurityKey BuildSecurityKey() => new RsaSecurityKey(new RSACryptoServiceProvider(2048))
{
    KeyId = "keyid"
};

public partial class Program { }