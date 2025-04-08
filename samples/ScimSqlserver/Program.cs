using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string connectionString = "";
builder.Services.AddScim()
    .UseEfStore((db) =>
    {
        db.UseSqlServer(connectionString, o =>
        {
            o.MigrationsAssembly("SimpleIdServer.Scim.SqlServerMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });
var app = builder.Build();
app.UseScim();
app.Run();