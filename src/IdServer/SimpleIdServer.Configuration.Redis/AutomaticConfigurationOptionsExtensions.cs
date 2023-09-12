// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Configuration.Redis;

public static class AutomaticConfigurationOptionsExtensions
{
    public static AutomaticConfigurationOptions UseRedisConnector(this AutomaticConfigurationOptions options, string configuration)
    {
        options.KeyValueConnector = new RedisKeyValueConnector(configuration);
        return options;
    }
}
