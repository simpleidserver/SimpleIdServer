using SimpleIdServer.Scim;

var builder = WebApplication.CreateBuilder(args);

const string connectionstring = "";
builder.Services.AddScim()
    .UseMongodbStorage(d =>
    {
        d.ConnectionString = connectionstring;
        d.Database = "scim";
        d.CollectionMappings = "mappings";
        d.CollectionRepresentations = "representations";
        d.CollectionSchemas = "schemas";
        d.SupportTransaction = false;
    });
var app = builder.Build();
app.UseScim();
app.Run();