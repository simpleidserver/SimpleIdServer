// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Sms;
using SimpleIdServer.IdServer.Startup;
using SimpleIdServer.IdServer.Store;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();
// RunInMemoryIdServer(builder.Services);
RunSqlServerIdServer(builder.Services);

var app = builder.Build();
SeedData(app);
app.UseSID()
    .UseWsFederation();
app.Run();

void RunInMemoryIdServer(IServiceCollection services)
{
    services.AddSIDIdentityServer()
        .UseInMemoryStore(o =>
        {
            o.AddInMemoryUsers(IdServerConfiguration.Users);
            o.AddInMemoryScopes(IdServerConfiguration.Scopes);
            o.AddInMemoryClients(IdServerConfiguration.Clients);
            o.AddInMemoryUMAResources(IdServerConfiguration.Resources);
            o.AddInMemoryUMAPendingRequests(IdServerConfiguration.PendingRequests);
        })
        .AddDeveloperSigningCredentials()
        .AddWsFederationSigningCredentials()
        .AddBackChannelAuthentication()
        .AddEmailAuthentication()
        .AddSmsAuthentication()
        // .EnableConfigurableAuthentication(IdServerConfiguration.Providers)
        .AddAuthentication(callback: (a) =>
        {
            /*
            a.AddWsAuthentication(o =>
            {
                o.MetadataAddress = "http://localhost:60001/FederationMetadata/2007-06/FederationMetadata.xml";
                o.Wtrealm = "urn:website";
                o.RequireHttpsMetadata = false;
            });
            */
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
                o.SignInScheme = SimpleIdServer.IdServer.Constants.DefaultExternalCookieAuthenticationScheme;
                o.AppId = "569242033233529";
                o.AppSecret = "12e0f33817634c0a650c0121d05e53eb";
            });
            /*
            a.Builder.AddOpenIdConnect(opts =>
            {
                opts.Authority = "http://localhost:8080/realms/master";
                opts.MetadataAddress = "http://localhost:8080/realms/master/.well-known/openid-configuration";
                opts.ClientId = "Sid";
                opts.ClientSecret = "yEoAQuAlubllnxBmdmXOBmS3GRKIr7bA";
                opts.ResponseType = "code";
                opts.UsePkce = true;
                opts.ResponseMode = "query";
                opts.SaveTokens = true;
                opts.GetClaimsFromUserInfoEndpoint = true;
                opts.RequireHttpsMetadata = false;
                opts.Scope.Add("openid");
                opts.Scope.Add("profile");
            });
            */
        })
        .AddWsFederation();
}

void RunSqlServerIdServer(IServiceCollection services)
{
    services.AddSIDIdentityServer(o =>
    {
        // o.IsEmailUsedDuringAuthentication = true;
    })
        .UseEFStore(o =>
        {
            o.UseSqlServer("Data Source=.;Initial Catalog=IdServer;Integrated Security=True;TrustServerCertificate=True", o =>
            {
                o.MigrationsAssembly("SimpleIdServer.IdServer.Startup");
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
        })
        .AddDeveloperSigningCredentials()
        .AddWsFederationSigningCredentials()
        .AddBackChannelAuthentication()
        .AddEmailAuthentication()
        .AddSmsAuthentication()
        // .EnableConfigurableAuthentication(IdServerConfiguration.Providers)
        .AddAuthentication(callback: (a) =>
        {
            /*
            a.AddWsAuthentication(o =>
            {
                o.MetadataAddress = "http://localhost:60001/FederationMetadata/2007-06/FederationMetadata.xml";
                o.Wtrealm = "urn:website";
                o.RequireHttpsMetadata = false;
            });
            */
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
                o.SignInScheme = SimpleIdServer.IdServer.Constants.DefaultExternalCookieAuthenticationScheme;
                o.AppId = "569242033233529";
                o.AppSecret = "12e0f33817634c0a650c0121d05e53eb";
            });
            /*
            a.Builder.AddOpenIdConnect("KeyCloak", "KeyCloak", opts =>
            {
                opts.Authority = "http://localhost:8080/realms/master";
                opts.MetadataAddress = "http://localhost:8080/realms/master/.well-known/openid-configuration";
                opts.ClientId = "Sid";
                opts.ClientSecret = "yEoAQuAlubllnxBmdmXOBmS3GRKIr7bA";
                opts.ResponseType = "code";
                opts.UsePkce = true;
                opts.ResponseMode = "query";
                opts.SaveTokens = true;
                opts.GetClaimsFromUserInfoEndpoint = true;
                opts.RequireHttpsMetadata = false;
                opts.Scope.Add("openid");
                opts.Scope.Add("profile");
            });
            */
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

            if (!dbContext.UmaPendingRequest.Any())
                dbContext.UmaPendingRequest.AddRange(IdServerConfiguration.PendingRequests);

            if (!dbContext.UmaResources.Any())
                dbContext.UmaResources.AddRange(IdServerConfiguration.Resources);

            if (!dbContext.AuthenticationSchemeProviders.Any())
                dbContext.AuthenticationSchemeProviders.AddRange(IdServerConfiguration.Providers);

            if (!dbContext.Acrs.Any())
            {
                dbContext.Acrs.Add(SimpleIdServer.IdServer.Constants.StandardAcrs.FirstLevelAssurance);
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Name = "email",
                    AuthenticationMethodReferences = new[] { "email" },
                    DisplayName = "Email authentication"
                });
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Name = "sms",
                    AuthenticationMethodReferences = new[] { "sms" },
                    DisplayName = "Sms authentication"
                });
                dbContext.Acrs.Add(new SimpleIdServer.IdServer.Domains.AuthenticationContextClassReference
                {
                    Name = "pwd-email",
                    AuthenticationMethodReferences = new[] { "pwd", "email" },
                    DisplayName = "Password and email authentication"
                });
            }

            dbContext.SaveChanges();
        }
    }
}