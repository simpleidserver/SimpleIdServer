// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Store;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
// RunInMemoryIdServer(builder.Services);
RunSqlServerIdServer(builder.Services);

var app = builder.Build();
SeedData(app);
app.UseSID();
app.Run();

void RunInMemoryIdServer(IServiceCollection services)
{
    services.AddSIDIdentityServer()
        .UseInMemoryStore(o =>
        {
            o.AddInMemoryUsers(IdServerConfiguration.Users);
            o.AddInMemoryScopes(IdServerConfiguration.Scopes);
            o.AddInMemoryClients(IdServerConfiguration.Clients);
        })
        .AddDeveloperSigningCredentials()
        .AddBackChannelAuthentication()
        // .EnableConfigurableAuthentication(IdServerConfiguration.Providers)
        .AddAuthentication(callback: (a) =>
        {
            a.AddOIDCAuthentication(opts =>
            {
                opts.Authority = "http://localhost:60001";
                opts.ClientId = "website";
                opts.ClientSecret = "password";
                opts.ResponseType = "code";
                opts.UsePkce = true;
                opts.ResponseMode = "query";
                opts.SaveTokens = true;
                opts.GetClaimsFromUserInfoEndpoint = true;
                opts.RequireHttpsMetadata = false;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
                opts.Scope.Add("profile");
            });
            a.Builder.AddFacebook(o =>
            {
                o.AppId = "569242033233529";
                o.AppSecret = "12e0f33817634c0a650c0121d05e53eb";
            });
        });
}

void RunSqlServerIdServer(IServiceCollection services)
{
    services.AddSIDIdentityServer()
        .UseEFStore(o =>
        {
            o.UseSqlServer("Data Source=.;Initial Catalog=IdServer;Integrated Security=True;TrustServerCertificate=True", o => o.MigrationsAssembly("SimpleIdServer.IdServer.Startup"));
        })
        .AddDeveloperSigningCredentials()
        .AddBackChannelAuthentication()
        // .EnableConfigurableAuthentication(IdServerConfiguration.Providers)
        .AddAuthentication(callback: (a) =>
        {
            a.AddOIDCAuthentication(opts =>
            {
                opts.Authority = "http://localhost:60001";
                opts.ClientId = "website";
                opts.ClientSecret = "password";
                opts.ResponseType = "code";
                opts.UsePkce = true;
                opts.ResponseMode = "query";
                opts.SaveTokens = true;
                opts.GetClaimsFromUserInfoEndpoint = true;
                opts.RequireHttpsMetadata = false;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };
                opts.Scope.Add("profile");
            });
            a.Builder.AddFacebook(o =>
            {
                o.AppId = "569242033233529";
                o.AppSecret = "12e0f33817634c0a650c0121d05e53eb";
            });
        });
}

void SeedData(WebApplication application)
{
    using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetService<StoreDbContext>())
        {
            dbContext.Database.Migrate();
            if (!dbContext.Scopes.Any())
                dbContext.Scopes.AddRange(IdServerConfiguration.Scopes);

            if (!dbContext.Users.Any())
                dbContext.Users.AddRange(IdServerConfiguration.Users);

            if (!dbContext.Clients.Any())
                dbContext.Clients.AddRange(IdServerConfiguration.Clients);

            if (!dbContext.AuthenticationSchemeProviders.Any())
                dbContext.AuthenticationSchemeProviders.AddRange(IdServerConfiguration.Providers);

            if (!dbContext.Acrs.Any())
                dbContext.Acrs.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.FirstLevelAssurance);

            dbContext.SaveChanges();
        }
    }
}