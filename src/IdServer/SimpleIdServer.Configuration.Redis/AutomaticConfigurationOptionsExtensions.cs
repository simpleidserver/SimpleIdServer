// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.DependencyInjection;

namespace SimpleIdServer.Configuration.Redis;

public static class AutomaticConfigurationOptionsExtensions
{
    public static AutomaticConfigurationOptions UseRedisConnector(this AutomaticConfigurationOptions options, string configuration)
    {
        options.Services.AddSingleton<IKeyValueConnector>(new RedisKeyValueConnector(configuration));
        options.KeyValueConnectorType = typeof(RedisKeyValueConnector);
        return options;
    }
}
