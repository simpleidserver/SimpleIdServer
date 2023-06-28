using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Website.Startup.Configurations;

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
}, o => ConfigureStorage(o));

void ConfigureStorage(DbContextOptionsBuilder b)
{
    var section = builder.Configuration.GetSection(nameof(StorageConfiguration));
    var conf = section.Get<StorageConfiguration>();
    switch (conf.Type)
    {
        case StorageTypes.SQLSERVER:
            b.UseSqlServer(conf.ConnectionString, o =>
            {
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            break;
        case StorageTypes.POSTGRE:
            b.UseNpgsql(conf.ConnectionString, o =>
            {
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            break;
    }
}

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