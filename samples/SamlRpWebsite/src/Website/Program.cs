using Microsoft.AspNetCore.Authentication;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "samlSp";
})
    .AddCookie("Cookies")
    .AddSamlSp("samlSp", options =>
    {
        options.SPId = "samlSp";
        options.IdpMetadataUrl = "https://localhost:5001/saml/metadata";
        var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        options.SigningCertificate = X509Certificate2.CreateFromPemFile(Path.Combine(currentPath, "sidClient.crt"), Path.Combine(currentPath, "sidClient.key"));
    });
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
