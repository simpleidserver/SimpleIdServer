// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Migrations;

public interface IMigrationServiceFactory
{
    IMigrationService Create(string name);
}

public class MigrationServiceFactory : IMigrationServiceFactory
{
    private readonly IEnumerable<IMigrationService> _migrationServices;

    public MigrationServiceFactory(IEnumerable<IMigrationService> migrationServices)
    {
        _migrationServices = migrationServices;
    }

    public IMigrationService Create(string name)
    {
        return _migrationServices.SingleOrDefault(m => m.Name == name);
    }
}
