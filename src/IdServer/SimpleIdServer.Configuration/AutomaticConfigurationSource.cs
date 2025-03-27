// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleIdServer.IdServer.Helpers;
using System;

namespace SimpleIdServer.Configuration;

public class AutomaticConfigurationSource : IConfigurationSource
{
    private readonly AutomaticConfigurationOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public AutomaticConfigurationSource()
    {

    }

    public AutomaticConfigurationSource(AutomaticConfigurationOptions options, IServiceProvider serviceProvider)
    {
        _options = options;
        _serviceProvider = serviceProvider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var keyValueConnector = _serviceProvider.GetRequiredService<IKeyValueConnector>();
        var realmStore = _serviceProvider.GetRequiredService<IRealmStore>();
        return new AutomaticConfigurationProvider(realmStore, _options, keyValueConnector);
    }
}