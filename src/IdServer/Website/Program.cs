using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

const string JWK = "{\"alg\":\"ES256\",\"crv\":\"P-256\",\"d\":\"mf1MvmivRY_TdH-J7gAt7ak4DYGnyLIqIZ3dgHL5NHk\",\"kid\":\"keyId\",\"kty\":\"EC\",\"use\":\"sig\",\"x\":\"MdwuTbn0TCQYgsER0-NeE3vtSx3H4HD9sSD7Zfkxt8k\",\"y\":\"ec27GOT5l3Mu8pzZsj6doPBNbCIp_5afjoP66qPfu4o\"}";
var jsonWebKey = JsonExtensions.DeserializeFromJson<JsonWebKey>(JWK);
var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=websiteFAPI.pfx"));

// Add services to the container.
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
        options.ResponseMode = "jwt";
        options.Authority = "https://localhost:5001/master";
        options.RequireHttpsMetadata = false;
        options.ClientId = "websiteFAPI";
        options.GetClaimsFromUserInfoEndpoint = true;
        options.SaveTokens = true;
        options.MTLSCertificate = null;
        options.ClientAuthenticationType = SimpleIdServer.OpenId.ClientAuthenticationTypes.TLS_CLIENT_AUTH;
        options.RequestType = SimpleIdServer.OpenId.RequestTypes.PAR;
        options.MTLSCertificate = certificate;
        options.SigningCredentials = new SigningCredentials(jsonWebKey, jsonWebKey.Alg);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    Secure = CookieSecurePolicy.Always
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
