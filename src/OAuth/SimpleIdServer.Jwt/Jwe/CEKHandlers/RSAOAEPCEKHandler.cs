// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jwe.CEKHandlers
{
    public class RSAOAEPCEKHandler : ICEKHandler
    {
        public string AlgName => ALG_NAME;
        public const string ALG_NAME = "RSA-OAEP";

        public byte[] Encrypt(byte[] payload, JsonWebKey jsonWebKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.Import(jsonWebKey.Content);
                return rsa.Encrypt(payload, RSAEncryptionPadding.OaepSHA1);
            }
        }

        public byte[] Decrypt(byte[] payload, JsonWebKey jsonWebKey)
        {
            using (var rsa = RSA.Create())
            {
                rsa.Import(jsonWebKey.Content);
                return rsa.Decrypt(payload, RSAEncryptionPadding.OaepSHA1);
            }
        }
    }
}
