// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;

namespace SimpleIdServer.Did.Key.Tests
{
    public class KeyIdentityDocumentBuilderFixture
    {
        [Test]
        public void When_Build_IdentityDocumentKey_JsonWebKey2020_Then_JSONIsCorrect()
        {
            var generateKey = SignatureKeyBuilder.NewES256K();
            var identityDocument = KeyIdentityDocumentBuilder.NewKey(generateKey)
                .AddVerificationMethod(generateKey)
                .Build();
            var json = identityDocument.Serialize();
            Assert.NotNull(json);
        }

        [Test]
        public void When_Build_IdentityDocumentKey_EcdsaPublicKeySecp256k1_Then_JSONIsCorrect()
        {
            var generateKey = SignatureKeyBuilder.NewES256K();
            var identityDocument = KeyIdentityDocumentBuilder.NewKey(generateKey)
                .AddVerificationMethod(generateKey, Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020)
                .Build();
            var json = identityDocument.Serialize();
            Assert.NotNull(json);
        }
    }
}
