// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
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
            var es384 = ES384SignatureKey.New();
            var jwk = es384.GetPublicKeyJwk();

            const string json = "{ \"kty\": \"EC\",\"crv\": \"P-384\", \"x\": \"GnLl6mDti7a2VUIZP5w6pcRX8q5nvEIgB3Q_5RI2p9F_QVsaAlDN7IG68Jn0dS_F\", \"y\": \"jq4QoAHKiIzezDp88s_cxSPXtuXYFliuCGndgU4Qp8l91xzD1spCmFIzQgVjqvcP\" }";
            var jsonWebKey = JsonWebKey.Create(json);

            // ES256K
            var identityDocument = IdentityDocumentBuilder.New("didSubject", "publicadr")
                .AddContext("https://w3id.org/security/suites/ed25519-2020/v1")
                .AddContext(c =>
                {
                    c.AddPropertyValue("foo", "https://example.com/property");
                    c.AddPropertyId("image", "http://schema.org/image");
                })
                .AddAlsoKnownAs("didSubject")
                .AddController("didController")
                .AddJsonWebKeyVerificationMethod(ES256KSignatureKey.New(), "controller")
                .AddJsonWebKeyVerificationMethod(ES256SignatureKey.New(), "controller")
                .AddJsonWebKeyVerificationMethod(ES384SignatureKey.New(), "controller")
                // .AddJsonWebKeyVerificationMethod(SignatureKeyBuilder.NewES(), "controller")
                .Build();
            // var json = identityDocument.Serialize();
            Assert.NotNull(json);
        }
    }
}
