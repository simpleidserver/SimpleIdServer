using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddAuthentication(o =>
{
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
}).AddWsFederation(options =>
{
    options.MetadataAddress = "http://localhost:6002/wsfed/metadata";
    options.RequireHttpsMetadata = false;
    options.Wtrealm = "rp1";
}).AddCookie();
builder.Services.AddAuthorization(c =>
{
    c.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
});
var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();