// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.InMemory;

public class DefaultRealmRepository : IRealmRepository
{
    private readonly List<Realm> _realmLst;

    public DefaultRealmRepository(List<Realm> realmLst)
    {
        _realmLst = realmLst;
    }

    public Task<Realm> Get(string name, CancellationToken cancellationToken)
        => Task.FromResult(_realmLst.SingleOrDefault(r => r.Name == name));
}
