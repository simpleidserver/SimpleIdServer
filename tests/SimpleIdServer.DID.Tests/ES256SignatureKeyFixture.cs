// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;

namespace SimpleIdServer.DID.Tests
{
    public class ES256SignatureKeyFixture
    {
        [Test]
        public void When_SignAndCheck_Signature_ThenTrueIsReturned()
        {
            // ARRANGE
            var privateKey = "040f1dbf0a2ca86875447a7c010b0fc6d39d76859c458fbe8f2bf775a40ad74a";
            var plaintext = "thequickbrownfoxjumpedoverthelazyprogrammer";
            var key = ES256SignatureKey.From(null, privateKey.HexToByteArray());

            // ACT
            var sig = key.Sign(plaintext);

            // ASSERT
            Assert.That("vOTe64WujVUjEiQrAlwaPJtNADx4usSlCfe8OXHS6Np1BqJdqdJX912pVwVlAjmbqR_TMVE5i5TWB_GJVgrHgg", Is.EqualTo(sig));
        }

        [Test]
        public void When_GenerateES256_Serialize_And_Deserialize_Then_NoInformationIsLost()
        {
            // ARRANGE
            var key = ES256SignatureKey.Generate();
            var jwk = key.GetPublicJwk();
            var newKey = ES256SignatureKey.From(jwk);

            // ACT
            var newJwk = newKey.GetPublicJwk();

            // ASSERT
            Assert.That(jwk.Kty, Is.EqualTo(newJwk.Kty));
            Assert.That(jwk.Crv, Is.EqualTo(newJwk.Crv));
            Assert.That(jwk.X, Is.EqualTo(newJwk.X));
            Assert.That(jwk.Y, Is.EqualTo(newJwk.Y));
        }
    }
}
