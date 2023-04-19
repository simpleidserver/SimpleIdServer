// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.Did.Jwt
{
    public class JwtValidator
    {
        // Implement : https://github.com/decentralized-identity/did-jwt/blob/87cbfd0f719fc2fdb0ac83ed9ca964b3c1b1b1a9/src/JWT.ts
        private static Dictionary<string, List<string>> pusupportedPublicKeyTypes = new Dictionary<string, List<string>>
        {
            { 
                "ES256K", new List<string> 
                { 
                    VerificationMethodTypes.EcdsaSecp256k1VerificationKey2019, 
                    VerificationMethodTypes.EcdsaSecp256k1RecoveryMethod2020, 
                    VerificationMethodTypes.Secp256k1VerificationKey2018, 
                    VerificationMethodTypes.Secp256k1SignatureVerificationKey2018,
                    VerificationMethodTypes.EcdsaPublicKeySecp256k1
                }
            },
            {
                "ES256K-R", new List<string>
                {

                }
            }
        };

        public void Validate(string json, IdentityDocument document)
        {
            var handler = new JsonWebTokenHandler();
            var jwt = handler.ReadJsonWebToken(json);
            if (string.IsNullOrWhiteSpace(jwt.Issuer)) throw new InvalidOperationException($"JWT iss is required");
            var did = jwt.Issuer;

        }
    }
}
