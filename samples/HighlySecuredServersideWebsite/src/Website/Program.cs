using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

const string JWK = "{\"alg\":\"ES256\",\"crv\":\"P-256\",\"d\":\"-U73QZxHx-pGMJhC9SG7Do4H0N4q6nvY4Fs2GGDVyFM\",\"kid\":\"keyId\",\"kty\":\"EC\",\"use\":\"sig\",\"x\":\"ikA6jQpF1OUjUw2wxXodnej6-LB7zVJZO7mlcIj9h0g\",\"y\":\"wED_dsFzH4YeJqrZqo_V-9B3gCAcwKg3N62oKenLkr8\"}";
var jsonWebKey = JsonExtensions.DeserializeFromJson<JsonWebKey>(JWK);
var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=websiteFAPI.pfx"));
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
        options.ClientAuthenticationType = SimpleIdServer.OpenIdConnect.ClientAuthenticationTypes.TLS_CLIENT_AUTH;
        options.RequestType = SimpleIdServer.OpenIdConnect.RequestTypes.PAR;
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
