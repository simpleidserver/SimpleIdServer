// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.IdServer;

public class IdServerStoreChooser
{
    public IdServerStoreChooser(IServiceCollection services, AuthenticationBuilder authBuilder)
    {
        Services = services;
        AuthBuilder = authBuilder;
    }

    public IServiceCollection Services { get; private set; }
    public AuthenticationBuilder AuthBuilder { get; private set; }
}
