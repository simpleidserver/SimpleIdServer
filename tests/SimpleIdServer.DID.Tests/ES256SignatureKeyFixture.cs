// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.DID.Tests;

public class ES256SignatureKeyFixture
{
    [Test]
    public void When_GenerateES256_Serialize_And_Deserialize_Then_NoInformationIsLost()
    {
        // ARRANGE
        var key = ES256SignatureKey.Generate();
        var jwk = key.GetPublicJwk();
        var newKey = ES256SignatureKey.From(jwk, null);

        // ACT
        var newJwk = newKey.GetPublicJwk();

        // ASSERT
        Assert.That(jwk.Kty, Is.EqualTo(newJwk.Kty));
        Assert.That(jwk.Crv, Is.EqualTo(newJwk.Crv));
        Assert.That(jwk.X, Is.EqualTo(newJwk.X));
        Assert.That(jwk.Y, Is.EqualTo(newJwk.Y));
    }
}
