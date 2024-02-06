// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.CredentialIssuer.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.Authority = "https://localhost:5001/master";
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters.ValidateAudience = false;
});
builder.Services.AddAuthorization(b =>
{
    b.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
    b.AddPolicy("credconfs", p => p.RequireClaim("scope", "credconfs"));
    b.AddPolicy("credinstances", p => p.RequireClaim("scope", "credinstances"));
});
builder.Services.AddLocalization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddCredentialIssuer()
    .UseInMemoryStore(c =>
    {
        c.AddCredentialConfigurations(CredentialIssuerConfiguration.CredentialConfigurations);
    });

var app = builder.Build();
app.UseStaticFiles();
app.UseRequestLocalization(e =>
{
    e.SetDefaultCulture("en-US");
    e.AddSupportedCultures("en-US");
    e.AddSupportedUICultures("en-US");
});
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();