// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer.Light.Startup;

public class SidServerSetup
{
    public static void ConfigureClientCredentials(IServiceCollection services)
    {
        services.AddSidIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryScopes(Config.Scopes);
    }
}
