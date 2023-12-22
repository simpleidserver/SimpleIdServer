// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;

namespace SimpleIdServer.DID.Tests;

public class Ed25519SignatureKeyFixture
{
    [Test]
    public void When_SignAndCheck_Signature_ThenTrueIsReturned()
    {
        // ARRANGE
        var privateKey = "9e55d1e1aa1f455b8baad9fdf975503655f8b359d542fa7e4ce84106d625b35206fac1f22240cffd637ead6647188429fafda9c9cb7eae43386ac17f61115075";
        var plaintext = "thequickbrownfoxjumpedoverthelazyprogrammer";
        var key = Ed25519SignatureKey.From(null, privateKey.HexToByteArray());

        // ACT
        var sig = key.Sign(plaintext);

        // ASSERT
        Assert.That("1y_N9v6xI4DyG9vIuloivxm91EV96nDM3HXBUI4P2Owk0IxazqX63rQ5jlBih6tP_4H5QhkHHqbree7ExmTBCw", Is.EqualTo(sig));
    }
}
