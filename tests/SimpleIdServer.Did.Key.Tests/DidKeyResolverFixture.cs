// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;

namespace SimpleIdServer.Did.Key.Tests;

public class DidKeyResolverFixture
{
    [Test]
    public void When_Resolve_DidKey_Then_DidDocument_Is_Valid()
    {
        // TODO
        // 
        // ARRANGE
        var key = "did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp";
        var resolver = DidKeyResolver.New();

        // ACT
        var didDocument = resolver.Resolve(key);

        // ASSERT
        var json = didDocument.Serialize();

        // TODO :https://github.com/decentralized-identity/did-key.rs/blob/eb00da6074d8bc61e5d4c8129fbdd9dc05735cbf/src/ed25519.rs#L5
        // TRY TO RESOLVE X2219 KEY FROM ED25119.



        Assert.IsNotNull(json);
    }
}