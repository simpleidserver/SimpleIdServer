// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;

namespace SimpleIdServer.Did.Key.Tests;

public class DidKeyGeneratorFixture
{
    [Test]
    public async Task When_Generate_DidKey_Then_DidDocument_Is_Correct()
    {
        // ARRANGE
        var generator = DidKeyGenerator.New();
        var resolver = DidKeyResolver.New();

        // ACT
        var did = generator.GenerateRandomES256KKey().Export(false, false);
        var resolved = await resolver.Resolve(did.Did, CancellationToken.None);

        // ASSERT
        Assert.That(resolved != null);
    }

    [Test]
    public async Task When_Generate_DidKey_Jwk_Then_DidDocument_Is_Correct()
    {
        // ARRANGE
        var generator = DidKeyGenerator.New();
        var resolver = DidKeyResolver.New();

        // ACT
        var did = generator.GenerateRandomES256KKey().Export(false, true);
        var resolved = await resolver.Resolve(did.Did, CancellationToken.None);

        // ASSERT
        Assert.That(resolved != null);
    }
}
