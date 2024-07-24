using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.OpenidFederation.Domains;
using SimpleIdServer.OpenidFederation.Store.EF;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
var signatureCredentials = new SigningCredentials(new RsaSecurityKey(RSA.Create()) { KeyId = "raId" }, SecurityAlgorithms.RsaSha256);

var jsonWebKey = signatureCredentials.SerializePublicJWK();
jsonWebKey.Alg = SecurityAlgorithms.RsaSha256;
jsonWebKey.Use = "sig";

builder.Services.AddDistributedMemoryCache();
builder.Services.AddRpFederation(r =>
{
    r.Client = new SimpleIdServer.IdServer.Domains.Client
    {
        ClientId = "http://localhost:7001",
        RedirectionUrls = new List<string>
        {
            "http://localhost:7001/signin-oidc"
        },
        ClientRegistrationTypesSupported = new List<string>
        {
            "automatic"
        },
        RequestObjectSigningAlg = SecurityAlgorithms.RsaSha256,
        Scopes = new List<Scope>
        {
            new Scope
            {
                Name = "openid"
            },
            new Scope
            {
                Name = "profile"
            }
        },
        ResponseTypes = new List<string>
        {
            "code"
        },
        GrantTypes = new List<string>
        {
            "authorization_code"
        },
        TokenEndPointAuthMethod = "private_key_jwt"
    };
    r.Client.Add(jsonWebKey.Kid, jsonWebKey, "sig", SecurityKeyTypes.RSA);
    r.SigningCredentials = signatureCredentials;
});
builder.Services.AddOpenidFederationStore();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "sid";
})
    .AddCookie("Cookies")
    .AddCustomOpenIdConnect("sid", options =>
    {

        options.SignInScheme = "Cookies";
        options.ResponseType = "code";
        options.Authority = "https://localhost:5001/master";
        options.RequireHttpsMetadata = false;
        options.ClientId = "http://localhost:7001";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
        options.UseFederationAutomaticRegistration(signatureCredentials);
    });

var app = builder.Build();
AddTrustedEntities(app.Services);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
static void AddTrustedEntities(IServiceProvider services)
{
    using (var scope = services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<OpenidFederationDbContext>();
        dbContext.FederationEntities.AddRange(new List<FederationEntity>
        {
            new FederationEntity
            {
                Id = Guid.NewGuid().ToString(),
                Sub = "http://localhost:7000",
                Realm = string.Empty,
                IsSubordinate = false
            }
        });
        dbContext.SaveChanges();
    }
}