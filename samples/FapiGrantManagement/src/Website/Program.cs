using System.Security.Cryptography.X509Certificates;
using Website;
using Website.Models;
using Website.Stores;

var builder = WebApplication.CreateBuilder(args);

var certificate = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "CN=fapiGrant.pfx"));
builder.Services.AddControllersWithViews();
builder.Services.Configure<WebsiteOptions>(o =>
{
    o.MTLSCertificate = certificate;
});
builder.Services.AddSingleton<IBankInfoStore>(new BankInfoStore(new List<BankInfo>
{
    new BankInfo { Name = "Bank", ClientId = "fapiGrant", AuthorizationUrl = "https://localhost:5001/master/authorization", ClientSecret = "password", TokenUrl = "https://localhost:5001/master/token" }
}));

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
