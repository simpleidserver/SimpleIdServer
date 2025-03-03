// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace EfdataSeeder;

public interface IDataMigrationService
{
    Task MigrateBeforeDeployment(CancellationToken cancellationToken);
    Task MigrateAfterDeployment(CancellationToken cancellationToken);
}

public class DataMigrationService : IDataMigrationService
{
    private readonly IEnumerable<IDataSeeder> _dataSeeders;

    public DataMigrationService(IServiceProvider serviceProvider)
    {
        _dataSeeders = serviceProvider.GetRequiredService<IEnumerable<IDataSeeder>>();
    }

    public async Task MigrateBeforeDeployment(CancellationToken cancellationToken)
    {
        foreach (var dataSeeder in _dataSeeders.Where(d => d.IsBeforeDeployment))
        {
            await dataSeeder.Apply(cancellationToken);
        }
    }

    public async Task MigrateAfterDeployment(CancellationToken cancellationToken)
    {
        foreach (var dataSeeder in _dataSeeders.Where(d => !d.IsBeforeDeployment))
        {
            await dataSeeder.Apply(cancellationToken);
        }
    }
}