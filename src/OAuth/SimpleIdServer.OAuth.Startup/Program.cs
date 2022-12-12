using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSIDIdentityServer();

var app = builder.Build();
app.MapControllers();
app.Run();