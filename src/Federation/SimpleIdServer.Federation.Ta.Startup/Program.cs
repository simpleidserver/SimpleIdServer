using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Federation.Ta.Startup;
using SimpleIdServer.OpenidFederation.Store.EF;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthorityFederation(o =>
{
    o.OrganizationName = "Trust anchor";
    o.SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "taId" }, SecurityAlgorithms.RsaSha256);
});
builder.Services.AddOpenidFederationStore();

var app = builder.Build();
AddTrustedEntities(app.Services);
app.MapControllers();
app.Run();

static void AddTrustedEntities(IServiceProvider services)
{
    using (var scope = services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OpenidFederationDbContext>();
        dbContext.FederationEntities.AddRange(TaConfiguration.FederationEntities);
        dbContext.SaveChanges();
    }
}