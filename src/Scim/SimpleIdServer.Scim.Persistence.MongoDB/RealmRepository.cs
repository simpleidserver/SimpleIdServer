// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB;

public class RealmRepository : IRealmRepository
{
    private readonly SCIMDbContext _scimDbContext;

    public RealmRepository(SCIMDbContext scimDbContext)
    {
        _scimDbContext = scimDbContext;
    }

    public async Task<Realm> Get(string name, CancellationToken cancellationToken)
    {
        var collection = _scimDbContext.Realms;
        var result = await collection.AsQueryable().Where(a => a.Name == name).ToMongoFirstAsync();
        return result;
    }
}
