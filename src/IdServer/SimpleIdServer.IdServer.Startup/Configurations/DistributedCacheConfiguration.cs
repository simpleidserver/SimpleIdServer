// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Startup.Configurations;

public class DistributedCacheConfiguration
{
    public DistributedCacheTypes Type { get; set; }
    public string? ConnectionString { get; set; } = null;
    public string? InstanceName { get; set; } = null;
}

public enum DistributedCacheTypes
{
    INMEMORY = 0,
    SQLSERVER = 1,
    REDIS = 2,
    POSTGRE = 3,
    MYSQL = 4
}
