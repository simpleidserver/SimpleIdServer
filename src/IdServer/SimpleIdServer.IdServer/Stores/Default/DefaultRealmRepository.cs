// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultRealmRepository : IRealmRepository
{
    private readonly List<Realm> _realms;

    public DefaultRealmRepository(List<Realm> realms)
    {
        _realms = realms;
    }

    public Task<List<Realm>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult(_realms);
    }

    public Task<Realm> Get(string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(_realms.SingleOrDefault(r => r.Name == name));
    }

    public void Add(Realm realm)
    {
        _realms.Add(realm);
    }

    public void Remove(Realm realm)
    {
        _realms.Remove(realm);
    }
}
