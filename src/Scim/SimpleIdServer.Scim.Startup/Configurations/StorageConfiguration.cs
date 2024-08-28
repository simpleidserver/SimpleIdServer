// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.Scim.Startup.Configurations
{
    public class StorageConfiguration
    {
        public StorageTypes Type { get; set; }
        public string ConnectionString { get; set; }
    }

    public enum StorageTypes
    {
        SQLSERVER = 0,
        POSTGRE = 1,
        MONGODB = 2,
        MYSQL = 3,
        SQLITE = 4
    }
}
