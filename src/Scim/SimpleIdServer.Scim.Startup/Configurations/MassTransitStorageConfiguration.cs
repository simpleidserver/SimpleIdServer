// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.Startup.Configurations;

public class MassTransitStorageConfiguration
{
    public bool IsEnabled { get; set; }
    public MassTransitStorageTypes Type { get; set; }
    public string ConnectionString { get; set; }
}

public enum MassTransitStorageTypes
{
    INMEMORY = 0,
    DIRECTORY = 1,
    AZURESTORAGE = 2
}
