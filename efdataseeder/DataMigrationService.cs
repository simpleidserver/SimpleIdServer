// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace EfdataSeeder;

public interface IDataMigrationService
{
    Task Migrate(CancellationToken cancellationToken);
}

public class DataMigrationService : IDataMigrationService
{
    private readonly IEnumerable<IDataSeeder> _dataSeeders;

    public DataMigrationService(IServiceProvider serviceProvider)
    {
        _dataSeeders = serviceProvider.GetRequiredService<IEnumerable<IDataSeeder>>();
    }

    public async Task Migrate(CancellationToken cancellationToken)
    {
        foreach (var dataSeeder in _dataSeeders)
        {
            await dataSeeder.Apply(cancellationToken);
        }
    }
}