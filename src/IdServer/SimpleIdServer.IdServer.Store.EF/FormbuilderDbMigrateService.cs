// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using DataSeeder;
using FormBuilder.EF;
using Microsoft.EntityFrameworkCore;

namespace SimpleIdServer.IdServer.Store.EF;

public class FormbuilderDbMigrateService : IDbMigrateService
{
    private readonly FormBuilderDbContext _dbContext;

    public FormbuilderDbMigrateService(FormBuilderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Migrate(CancellationToken cancellationToken)
    {
        if (_dbContext.Database.IsInMemory())
        {
            return Task.CompletedTask;
        }

        return _dbContext.Database.MigrateAsync(cancellationToken);
    }
}
