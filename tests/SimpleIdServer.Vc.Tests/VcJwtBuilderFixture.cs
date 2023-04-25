// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.Vc.Jwt;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc.Tests
{
    public class VcJwtBuilderFixture
    {
        [Test]
        public void When_Build_VcJWT_Then_SignatureIsCorrect()
        {
            const string credentialSubject = "{\"degree\": { \"type\": \"BachelorDegree\", \"name\": \"Baccalauréat en musiques numériques\" }}";
            var generateKey = SignatureKeyBuilder.NewES256K();
            var hex = generateKey.PrivateKey.ToHex();
            var identityDocument = IdentityDocumentBuilder.New("did", "publicadr")
                .AddVerificationMethod(generateKey, Did.Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019)
                .Build();
            var verifiableCredential = VerifiableCredentialBuilder
                .New()
                .SetCredentialSubject(JsonObject.Parse(credentialSubject).AsObject()).Build();
            var jwt = VcJwtBuilder.GenerateToken(identityDocument, verifiableCredential, hex.HexToByteArray(), "did#delegate-1");
            var isValid = DidJwtValidator.Validate(jwt, identityDocument, Did.Models.KeyPurposes.VerificationKey);
            Assert.IsTrue(isValid);
        }
    }
}
