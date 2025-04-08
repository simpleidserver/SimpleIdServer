using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string connectionString = "";
builder.Services.AddScim().UseEfStore(e =>
{
    e.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), o =>
    {
        o.MigrationsAssembly("SimpleIdServer.Scim.MySQLMigrations");
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        o.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Ignore);
    });
});
var app = builder.Build();
app.UseScim();
app.Run();