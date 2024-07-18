var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRpFederation(r =>
{
    r.Client = new SimpleIdServer.IdServer.Domains.Client
    {

    };
});