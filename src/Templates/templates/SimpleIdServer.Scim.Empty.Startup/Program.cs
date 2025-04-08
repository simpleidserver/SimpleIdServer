var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScim();
var app = builder.Build();
app.UseScim();
app.Run();