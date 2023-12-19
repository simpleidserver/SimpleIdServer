// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;

namespace SimpleIdServer.DID.Tests;

public class ES256KSignatureKeyFixture
{
    [Test]
    public void When_SignAndCheck_Signature_ThenTrueIsReturned()
    {
        // ARRANGE
        var privateKey = "278a5de700e29faae8e40e366ec5012b5ec63d36ec77e8a2417154cc1d25383f";
        var plaintext = "thequickbrownfoxjumpedoverthelazyprogrammer";
        var key = ES256KSignatureKey.From(null, privateKey.HexToByteArray());

        // ACT
        var sig = key.Sign(plaintext);

        // ASSERT
        Assert.That("jsvdLwqr-O206hkegoq6pbo7LJjCaflEKHCvfohBP9XJ4C7mG2TPL9YjyKEpYSXqqkUrfRoCxQecHR11Uh7POw", Is.EqualTo(sig));
    }

    [Test]
    public void When_GenerateES256K_Serialize_And_Deserialize_Then_NoInformationIsLost()
    {
        // ARRANGE
        var key = ES256KSignatureKey.Generate();
        var jwk = key.GetPublicJwk();
        var newKey = ES256KSignatureKey.From(jwk);

        // ACT
        var newJwk = newKey.GetPublicJwk();

        // ASSERT
        Assert.That(jwk.Kty, Is.EqualTo(newJwk.Kty));
        Assert.That(jwk.Crv, Is.EqualTo(newJwk.Crv));
        Assert.That(jwk.X, Is.EqualTo(newJwk.X));
        Assert.That(jwk.Y, Is.EqualTo(newJwk.Y));
    }
}
