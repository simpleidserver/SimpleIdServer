// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.DID.Tests;

public class RSASignatureKeyFixture
{
    [Test]
    public void When_GenerateRSA_Serialize_And_Deserialize_Then_NoInformationIsLost()
    {
        // ARRANGE
        var key = RSASignatureKey.Generate();
        var jwk = key.GetPublicJwk();
        var newKey = RSASignatureKey.From(jwk, null);

        // ACT
        var publicJwk = newKey.GetPublicJwk();

        // ASSERT
        Assert.That(jwk.N, Is.EqualTo(publicJwk.N));
        Assert.That(jwk.E, Is.EqualTo(publicJwk.E));
    }
}