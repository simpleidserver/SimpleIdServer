// Copyright (c) SimpleIdServer. AllClients rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace SimpleIdServer.IdServer.Template.Startup.Configurations;

public class DuendeMigrationOptions
{
    public StorageTypes Transport
    {
        get; set;
    }

    public string ConnectionString
    {
        get; set;
    }
}
