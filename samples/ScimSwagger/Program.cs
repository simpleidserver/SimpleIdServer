using SimpleIdServer.Scim;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScim().EnableSwagger();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SCIM API V1");
});
app.UseScim();
app.Run();