// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did;

public interface IDidFactoryResolver
{
    Task<DidDocument> Resolve(string did, CancellationToken cancellationToken);
}

public class DidFactoryResolver : IDidFactoryResolver
{
    private readonly IEnumerable<IDidResolver> _resolvers;

    public DidFactoryResolver(IEnumerable<IDidResolver> resolvers)
    {
        _resolvers = resolvers;
    }

    public async Task<DidDocument> Resolve(string did, CancellationToken cancellationToken)
    {
        var decentralizedIdentifier = DidExtractor.Extract(did);
        var resolver = _resolvers.SingleOrDefault(r => r.Method == decentralizedIdentifier.Method);
        if (resolver == null) throw new InvalidOperationException($"the method {decentralizedIdentifier.Method} doesn't exist");
        return await resolver.Resolve(did, cancellationToken);
    }
}
