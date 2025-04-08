using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string connectionstring = "";
builder.Services.AddScim().UseEfStore(e =>
{
    e.UseSqlite(connectionstring, o =>
    {
        o.MigrationsAssembly("SimpleIdServer.Scim.SqliteMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
}, c =>
{
    c.IgnoreBulkOperation = true;
});
var app = builder.Build();
app.UseScim();
app.Run();