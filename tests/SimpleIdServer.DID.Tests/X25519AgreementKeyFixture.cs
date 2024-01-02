// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.DID.Tests;

public class X25519AgreementKeyFixture
{
    [Test]
    public void When_GenerateX25519_Serialize_And_Deserialize_Then_NoInformationIsLost()
    {
        // ARRANGE
        var key = X25519AgreementKey.Generate();
        var jwk = key.GetPublicJwk();
        var newKey = X25519AgreementKey.From(jwk, null);

        // ACT
        var newJwk = newKey.GetPublicJwk();

        // ASSERT
        Assert.That(jwk.Kty, Is.EqualTo(newJwk.Kty));
        Assert.That(jwk.Crv, Is.EqualTo(newJwk.Crv));
        Assert.That(jwk.X, Is.EqualTo(newJwk.X));
    }
}
