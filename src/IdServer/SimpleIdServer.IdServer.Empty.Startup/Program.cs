// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
var builder = WebApplication.CreateBuilder(args);

var scope = ScopeBuilder.CreateApiScope("api1", false).Build();
var clients = new List<Client>
{
    ClientBuilder.BuildApiClient("client", "secret").AddScope(scope).Build()
};
var scopes = new List<Scope>
{
    scope
};
builder.AddSidIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryClients(clients)
    .AddInMemoryScopes(scopes);

var app = builder.Build();
app.UseSid();
app.Run();