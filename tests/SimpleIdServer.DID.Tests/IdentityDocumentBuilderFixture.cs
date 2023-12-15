// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.DID.Tests
{
    public class IdentityDocumentBuilderFixture
    {
        [Test]
        public void When_Build_IdentityDocument_Then_JSONIsCorrect()
        {
            // X25519
            var identityDocument = IdentityDocumentBuilder.New("didSubject", "publicadr")
                .AddContext("https://w3id.org/security/suites/ed25519-2020/v1")
                .AddContext(c =>
                {
                    c.AddPropertyValue("foo", "https://example.com/property");
                    c.AddPropertyId("image", "http://schema.org/image");
                })
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                // .AddJsonWebKeyVerificationMethod(Ed25519SignatureKey.New(), "controller")
                .AddJsonWebKeyVerificationMethod(ES256KSignatureKey.New(), "controller")
                .AddJsonWebKeyVerificationMethod(ES256SignatureKey.New(), "controller")
                .AddJsonWebKeyVerificationMethod(ES384SignatureKey.New(), "controller")
                .AddJsonWebKeyVerificationMethod(RSA2048SignatureKey.New(), "controller")
                .AddPublicKeyMultibaseVerificationMethod(ES256KSignatureKey.New(), "controller")
                .Build();
            var json = identityDocument.Serialize();
            Assert.NotNull(json);
        }
    }
}
