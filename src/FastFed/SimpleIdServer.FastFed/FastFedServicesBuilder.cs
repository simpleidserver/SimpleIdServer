// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using System;

namespace SimpleIdServer.FastFed;

public class FastFedServicesBuilder
{
    internal FastFedServicesBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; private set; }
}
