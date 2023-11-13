var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSIDWebsite(o =>
{
    o.IdServerBaseUrl = builder.Configuration["IdServerBaseUrl"];
    o.SCIMUrl = builder.Configuration["ScimBaseUrl"];
    o.IsReamEnabled = bool.Parse(builder.Configuration["IsRealmEnabled"]);
});

builder.Services.AddDefaultSecurity(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(edps =>
{
    edps.MapBlazorHub();
    edps.MapFallbackToPage("/_Host");
    edps.MapControllers();
});
app.Run();