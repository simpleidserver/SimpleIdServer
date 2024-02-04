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
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCredentialIssuer()
    .UseInMemoryStore(c =>
    {
        c.AddCredentialConfigurations(CredentialIssuerConfiguration.CredentialConfigurations);
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();