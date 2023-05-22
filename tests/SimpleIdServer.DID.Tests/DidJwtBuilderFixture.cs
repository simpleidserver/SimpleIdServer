// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Jwt;

namespace SimpleIdServer.DID.Tests
{
    public class DidJwtBuilderFixture
    {
        [Test]
        public void When_Build_JWT_Then_SignatureIsCorrect()
        {
            var generateKey = SignatureKeyBuilder.NewES256K();
            var hex = generateKey.PrivateKey.ToHex();
            var identityDocument = IdentityDocumentBuilder.New("did", "publicadr")
                .AddVerificationMethod(generateKey, SimpleIdServer.Did.Constants.VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019)
                .Build();
            var dd = identityDocument.Serialize();

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
    }
}
