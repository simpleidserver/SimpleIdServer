using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// ConfigureMTLS();
ConfigureDPoP();

void ConfigureMTLS()
{
    var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=websiteFAPI.pfx"));
    var serializedJwk = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "JWK.json")); 
    var jsonWebKey = JsonExtensions.DeserializeFromJson<JsonWebKey>(serializedJwk);
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
            options.ClientId = "websiteFAPIMTLS";
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;
            options.UseMTLSProof(certificate, new SigningCredentials(jsonWebKey, jsonWebKey.Alg));
        });

}

void ConfigureDPoP()
{
    var serializedJwk = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "JWK.json"));
    var jwk = JsonExtensions.DeserializeFromJson<JsonWebKey>(serializedJwk);
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
            options.ClientId = "websiteFAPIDPoP";
            options.GetClaimsFromUserInfoEndpoint = true;
            options.SaveTokens = true;
            options.UseDPoPProof(new SigningCredentials(jwk, jwk.Alg));
        });
}

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
