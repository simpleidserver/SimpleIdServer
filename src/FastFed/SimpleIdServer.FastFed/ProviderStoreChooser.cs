// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.FastFed;

public class ProviderStoreChooser
{
    public ProviderStoreChooser(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; private set; }
}
