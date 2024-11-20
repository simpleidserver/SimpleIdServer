// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informa
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Seeding;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Seeding;

/// <summary>
/// Implements the method that allows to seed records of scopes.
/// </summary>
public class ScopeEntitySeeder : IEntitySeeder<ScopeSeedDto>
{
    private readonly ILogger<ScopeEntitySeeder> _logger;
    private readonly DbContext _dbContext;

    public ScopeEntitySeeder(ILogger<ScopeEntitySeeder> logger, DbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SeedAsync(IReadOnlyCollection<ScopeSeedDto> records, CancellationToken cancellationToken = default)
    {
        string[] dbLoginNames = await _dbContext.Client.Queryable<SugarScope>()
            .Select(u => u.Name.ToUpper())
            .ToArrayAsync();

        ScopeSeedDto[] scopesNotInDb = (from r in records
                                        join ln in dbLoginNames on r.Name.ToUpper() equals ln
                                        into qry
                                        from l_join in qry.DefaultIfEmpty()
                                        where l_join == null
                                        select r).ToArray();

        if (scopesNotInDb.Length > 0)
        {
            var scopesToCreate = new List<SugarScope>();

            foreach (ScopeSeedDto scope in scopesNotInDb)
            {
                Realm? realm = null;

                if (!string.IsNullOrEmpty(scope.Realm))
                {
                    SugarRealm sugarRealm = await _dbContext.Client.Queryable<SugarRealm>()
                        .FirstAsync(r => r.RealmsName.ToUpper() == scope.Realm.ToUpper(), cancellationToken);

                    realm = sugarRealm?.ToDomain();
                }

                ScopeBuilder builder = ScopeBuilder.Create(scope.Name, scope.IsExposedInConfigurationEdp, realm)
                        .SetDescription(scope.Description)
                        .SetComponent(scope.Component);

                if (!string.IsNullOrEmpty(scope.Type) && Enum.TryParse(scope.Type, out ScopeTypes type))
                {
                    builder.SetType(type);
                }

                if (!string.IsNullOrEmpty(scope.Protocol) && Enum.TryParse(scope.Protocol, out ScopeProtocols protocol))
                {
                    builder.SetProtocol(protocol);
                }

                if (!string.IsNullOrEmpty(scope.ComponentAction) && Enum.TryParse(scope.ComponentAction, out ComponentActions componentAction))
                {
                    builder.SetComponentAction(componentAction);
                }

                SugarScope sugarScope = SugarScope.Transform(builder.Build());

                scopesToCreate.Add(sugarScope);
            }

            await _dbContext.Client.Insertable<SugarScope>(scopesToCreate).ExecuteCommandAsync(cancellationToken);
        }

        _logger.LogInformation("{count} scopes seeded.", scopesNotInDb.Length);
    }
}
