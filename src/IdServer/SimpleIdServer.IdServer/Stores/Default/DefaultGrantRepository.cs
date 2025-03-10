// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Stores.Default;

public class DefaultGrantRepository : IGrantRepository
{
    private readonly List<Consent> _consents;

    public DefaultGrantRepository(List<Consent> consents)
    {
        _consents = consents;
    }

    public Task<Consent> Get(string id, CancellationToken cancellation)
        => Task.FromResult(_consents.SingleOrDefault(g => g.Id == id));

    public void Remove(Consent consent) => _consents.Remove(consent);
}
