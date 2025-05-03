// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace IdServerLogging.Configurations
{
    public class StorageConfiguration
    {
        public StorageTypes Type { get; set; }
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public enum StorageTypes
    {
        INMEMORY = 0,
        SQLSERVER = 1,
        POSTGRE = 2,
        MYSQL = 3,
        SQLITE = 4
    }
}