// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Key;

namespace SimpleIdServer.Did.Jwt.Tests
{
    public class DidJwtBuilderFixture
    {
        [Test]
        public void When_Build_JWT_Then_SignatureIsCorrect()
        {
            var generateKey = SignatureKeyBuilder.NewES256K();
            var hex = generateKey.PrivateKey.ToHex();
            var identityDocument = DidDocumentBuilder.New("did")
                // .AddVerificationMethod(generateKey, SimpleIdServer.Did.Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019)
                .Build();
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    { "claim", "value" }
                },
                Issuer = "did"
            };
            var jwt = DidJwtBuilder.GenerateToken(securityTokenDescriptor, identityDocument, hex, "did#delegate-1");
            var isValid = DidJwtValidator.Validate(jwt, identityDocument, Did.Models.KeyPurposes.VerificationKey);
            Assert.True(isValid);
        }

        [Test]
        public void When_Build_JWT_WithDIDKey_Then_SignatureIsCorrect()
        {
            /*
             * var generateKey = SignatureKeyBuilder.NewED25519();
            var hex = generateKey.PrivateKey.ToHex();
            var identityDocument = KeyIdentityDocumentBuilder.NewKey(generateKey)
                .AddVerificationMethod(generateKey, Did.Constants.VerificationMethodTypes.Ed25519VerificationKey2020)
                .Format();
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    { "claim", "value" }
                },
                Issuer = "did"
            };
            var jwt = DidJwtBuilder.GenerateToken(securityTokenDescriptor, identityDocument, hex);
            var isValid = DidJwtValidator.Validate(jwt, identityDocument, Did.Models.KeyPurposes.VerificationKey);
            Assert.True(isValid);
            */
        }
    }
}
