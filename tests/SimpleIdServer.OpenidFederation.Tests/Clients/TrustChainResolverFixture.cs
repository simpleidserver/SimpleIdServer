// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.OpenidFederation.Clients;
using System.Collections.Concurrent;

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
        var validationResult = trustChain.Select(v => v.Validate());

        // ACT
        Assert.IsNotNull(trustChain);
        Assert.IsNotNull(validationResult);
        Assert.IsFalse(validationResult.SelectMany(v => v.ErrorMessages).Any());
    }

    [Test]
    public void When_Transform_Dic_To_TrustChains_Then_ListIsCorrect()
    {
        // ARRANGE
        var dic = new ConcurrentDictionary<string, EntityStatement>();
        dic.TryAdd("key1", new EntityStatement(null, null));
        dic.TryAdd("key1;key2", new EntityStatement(null, null));
        dic.TryAdd("key1;key3", new EntityStatement(null, null));
        dic.TryAdd("key1;key2;key4", new EntityStatement(null, null));
        dic.TryAdd("key1;key3;key5", new EntityStatement(null, null));
        dic.TryAdd("key1;key3;key6", new EntityStatement(null, null));

        // ACT
        var trustChains = TrustChainResolver.Transform(dic);

        // ASSERT
        Assert.IsNotNull(trustChains);
        Assert.IsTrue(trustChains.Any(c => c.Path == "key1;key2;key4"));
        Assert.IsTrue(trustChains.Any(c => c.Path == "key1;key3;key5"));
        Assert.IsTrue(trustChains.Any(c => c.Path == "key1;key3;key6"));
        Assert.AreEqual(3, trustChains.Count());
        Assert.IsTrue(trustChains.All(c => c.EntityStatements.Count() == 3));
    }
}