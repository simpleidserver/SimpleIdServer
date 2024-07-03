// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.OpenidFederation.Clients;

namespace SimpleIdServer.OpenidFederation.Tests.Clients;

public class TrustChainResolverFixture
{
    [Test]
    public async Task When_Resolve_TrustChain_Then_NoExceptionIsThrown()
    {
        // ARRANGE
        const string entityId = "https://relying-party.authlete.net/6298746299842758";
        var resolver = TrustChainResolver.New();

        // ACT
        var trustChain = await resolver.ResolveTrustChains(entityId, CancellationToken.None);
        var validationResult = trustChain.Validate();

        // ACT
        Assert.IsNotNull(trustChain);
        Assert.IsNotNull(validationResult);
    }
}