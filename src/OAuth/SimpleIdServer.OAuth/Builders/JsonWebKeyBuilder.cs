// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SimpleIdServer.OAuth.Builders
{
    public class JsonWebKeyBuilder
    {
        private JsonWebKeyBuilder() { }

        public static JsonWebKey BuildRSA(string keyId, string alg = SecurityAlgorithms.RsaSha256)
        {
            var rsa = RSA.Create(2048);
            RsaSecurityKey securityKey = new(rsa)
            {
                KeyId = keyId
            };
            var result = JsonWebKeyConverter.ConvertFromRSASecurityKey(securityKey);
            result.Alg = alg;
            return result;
        }
    }
}
