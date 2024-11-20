// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license informa
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Seeding;

namespace SimpleIdServer.IdServer.Store.EF.Seeding
{
    /// <summary>
    /// Implements the method that allows to seed records of scopes.
    /// </summary>
    public class ScopeEntitySeeder : IEntitySeeder<ScopeSeedDto>
    {
        private readonly ILogger<ScopeEntitySeeder> _logger;
        private readonly StoreDbContext _storeDbContext;

        public ScopeEntitySeeder(ILogger<ScopeEntitySeeder> logger, StoreDbContext storeDbContext)
        {
            _logger = logger;
            _storeDbContext = storeDbContext;
        }

        public async Task SeedAsync(IReadOnlyCollection<ScopeSeedDto> records, CancellationToken cancellationToken = default)
        {
            string[] dbScopeNames = await _storeDbContext.Scopes.Select(u => u.Name.ToUpper())
                .ToArrayAsync(cancellationToken);

            ScopeSeedDto[] scopesNotInDb = (from r in records
                                            join ln in dbScopeNames on r.Name.ToUpper() equals ln
                                            into qry
                                            from l_join in qry.DefaultIfEmpty()
                                            where l_join == null
                                            select r).ToArray();

            if (scopesNotInDb.Length > 0)
            {
                var scopesToCreate = new List<Scope>();

                foreach (ScopeSeedDto scope in scopesNotInDb)
                {
                    Realm? realm = null;

                    if (!string.IsNullOrEmpty(scope.Realm))
                    {
                        realm = await _storeDbContext.Realms
                            .FirstOrDefaultAsync(r => r.Name.ToUpper() == scope.Realm.ToUpper(), cancellationToken);
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

                    scopesToCreate.Add(builder.Build());
                }

                await _storeDbContext.Scopes.AddRangeAsync(scopesToCreate, cancellationToken);
                await _storeDbContext.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation("{count} scopes seeded.", scopesNotInDb.Length);
        }
    }
}
