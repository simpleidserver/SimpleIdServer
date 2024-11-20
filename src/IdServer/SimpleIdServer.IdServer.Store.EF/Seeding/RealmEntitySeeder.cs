// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informa
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Seeding;

namespace SimpleIdServer.IdServer.Store.EF.Seeding;

/// <summary>
/// Implements the method that allows to seed records of realms.
/// </summary>
public class RealmEntitySeeder : IEntitySeeder<RealmSeedDto>
{
    private readonly ILogger<RealmEntitySeeder> _logger;
    private readonly StoreDbContext _storeDbContext;

    public RealmEntitySeeder(ILogger<RealmEntitySeeder> logger, StoreDbContext storeDbContext)
    {
        _logger = logger;
        _storeDbContext = storeDbContext;
    }

    public async Task SeedAsync(IReadOnlyCollection<RealmSeedDto> records, CancellationToken cancellationToken = default)
    {
        string[] dbRealmNames = await _storeDbContext.Realms.Select(r => r.Name.ToUpper())
            .ToArrayAsync(cancellationToken);

        RealmSeedDto[] realmsNotInDb = (from r in records
                                      join ln in dbRealmNames on r.Name.ToUpper() equals ln
                                      into qry
                                      from l_join in qry.DefaultIfEmpty()
                                      where l_join == null
                                      select r).ToArray();

        if (realmsNotInDb.Length > 0)
        {
            var realmsToCreate = new List<Realm>();

            foreach (var realm in realmsNotInDb)
            {
                string description = !string.IsNullOrEmpty(realm.Description) ? realm.Description : realm.Name;
                RealmBuilder builder = RealmBuilder.Create(realm.Name, description);

                realmsToCreate.Add(builder.Build());
            }

            await _storeDbContext.Realms.AddRangeAsync(realmsToCreate, cancellationToken);
            await _storeDbContext.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("{count} realms seeded.", realmsNotInDb.Length);
    }
}
