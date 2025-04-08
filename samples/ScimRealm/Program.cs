var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScim()
    .EnableRealm();
var app = builder.Build();
app.UseScim();
app.Run();