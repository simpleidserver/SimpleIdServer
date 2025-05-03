// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdServerLogging.Configurations;

public class KeyValueConfiguration
{
    public KeyValueTypes Type
    {
        get; set;
    }

    public string ConnectionString
    {
        get; set;
    }
}

public enum KeyValueTypes
{
    INMEMORY = 0,
    REDIS = 1
}
