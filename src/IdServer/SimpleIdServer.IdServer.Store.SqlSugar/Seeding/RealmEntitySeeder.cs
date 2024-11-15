// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informa
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Seeding;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Seeding;

/// <summary>
/// Implements the method that allows to seed records of realms.
/// </summary>
public class RealmEntitySeeder : IEntitySeeder<RealmSeedDto>
{
    private readonly ILogger<RealmEntitySeeder> _logger;
    private readonly DbContext _dbContext;

    public RealmEntitySeeder(ILogger<RealmEntitySeeder> logger, DbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SeedAsync(IReadOnlyCollection<RealmSeedDto> records, CancellationToken cancellationToken = default)
    {
        string[] dbRealmNames = await _dbContext.Client.Queryable<SugarRealm>()
            .Select(u => u.RealmsName.ToUpper())
            .ToArrayAsync();

        RealmSeedDto[] realmsNotInDb = (from r in records
                                      join ln in dbRealmNames on r.Name.ToUpper() equals ln
                                      into qry
                                      from l_join in qry.DefaultIfEmpty()
                                      where l_join == null
                                      select r).ToArray();

        if (realmsNotInDb.Length > 0)
        {
            var realmsToCreate = new List<SugarRealm>();

            foreach (RealmSeedDto realm in realmsNotInDb)
            {
                string description = !string.IsNullOrEmpty(realm.Description) ? realm.Description : realm.Name;
                RealmBuilder builder = RealmBuilder.Create(realm.Name, description);
                SugarRealm sugarRealm = SugarRealm.Transform(builder.Build());

                realmsToCreate.Add(sugarRealm);
            }

            await _dbContext.Client.Insertable<SugarRealm>(realmsToCreate).ExecuteCommandAsync(cancellationToken);
        }

        _logger.LogInformation("{count} realms seeded.", realmsNotInDb.Length);
    }
}
