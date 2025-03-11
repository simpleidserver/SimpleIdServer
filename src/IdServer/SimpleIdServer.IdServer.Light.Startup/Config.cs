// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Light.Startup;

public class Config
{
    public static readonly Scope Scope = ScopeBuilder.CreateApiScope("api1", false).Build();

    public static List<Client> Clients => new List<Client>
    {
        ClientBuilder.BuildApiClient("client", "secret").AddScope(Scope).Build()
    };

    public static List<Scope> Scopes => new List<Scope>
    {
        Scope
    };

    public static List<User> Users => new List<User>
    {
        UserBuilder.Create("administrator", "password").Build()
    };
}