// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;

namespace SimpleIdServer.Did.Key.Tests;

public class DidKeyResolverFixture
{
    [Test]
    public void When_Resolve_DidKey_Then_DidDocument_Is_Valid()
    {
        // ARRANGE
        var key = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var resolver = DidKeyResolver.New();

        // ACT
        var didDocument = resolver.Resolve(key);

        // ASSERT
        var json = didDocument.Serialize();
        Assert.IsNotNull(json);
    }
}