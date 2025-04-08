using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
const string connectionstring = "";
builder.Services.AddScim()
    .UseEfStore(o =>
    {
        o.UseNpgsql(connectionstring, o =>
        {
            o.MigrationsAssembly("SimpleIdServer.Scim.PostgreMigrations");
            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });
    });
var app = builder.Build();
app.UseScim();
app.Run();