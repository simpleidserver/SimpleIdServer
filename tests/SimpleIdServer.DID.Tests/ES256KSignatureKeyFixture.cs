// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Jwt.Crypto;

namespace SimpleIdServer.DID.Tests
{
    public class ES256KSignatureKeyFixture
    {
        [Test]
        public void When_SignAndCheck_Signature_ThenTrueIsReturned()
        {
            var privateKey = "278a5de700e29faae8e40e366ec5012b5ec63d36ec77e8a2417154cc1d25383f";
            var plaintext = "thequickbrownfoxjumpedoverthelazyprogrammer";
            var key = new ES256KSignatureKey(null, privateKey.HexToByteArray());
            var signature = key.Sign(plaintext);
            Assert.True(key.Check(plaintext, signature));
        }
    }
}
