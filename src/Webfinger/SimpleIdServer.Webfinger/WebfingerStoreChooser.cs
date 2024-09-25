// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Webfinger;

public class WebfingerStoreChooser
{
    public WebfingerStoreChooser(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; private set; }
}
