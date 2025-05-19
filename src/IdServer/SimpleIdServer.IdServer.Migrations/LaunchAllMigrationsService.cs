// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Migrations;

public interface ILaunchAllMigrationsService
{
    Task LaunchAll(string realm, CancellationToken cancellationToken);
}

public class LaunchAllMigrationsService : ILaunchAllMigrationsService
{
    private readonly IEnumerable<IMigrationService> _migrationServices;
    private readonly ILaunchMigrationService _launchMigrationService;

    public LaunchAllMigrationsService(IEnumerable<IMigrationService> migrationServices, ILaunchMigrationService launchMigrationService)
    {
        _migrationServices = migrationServices;
        _launchMigrationService = launchMigrationService;
    }

    public async Task LaunchAll(string realm, CancellationToken cancellationToken)
    {
        var names = _migrationServices.Select(m => m.Name);
        foreach(var name in names)
        {
            await _launchMigrationService.Launch(realm, name, cancellationToken);
        }
    }
}
