// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public abstract class PSSignHandler : ISignHandler
    {
        public string Sign(string payload, JsonWebKey jsonWebKey)
        {
            var bytesToBeSigned = ASCIIEncoding.ASCII.GetBytes(payload);
            using (var rsa = RSA.Create())
            {
                rsa.Import(jsonWebKey.Content);
                return rsa.SignData(bytesToBeSigned, Alg, RSASignaturePadding.Pss).Base64EncodeBytes();
            }
        }

        public bool Verify(string payload, byte[] signature, JsonWebKey jsonWebKey)
        {
            var plainBytes = ASCIIEncoding.ASCII.GetBytes(payload);
            using (var rsa = RSA.Create())
            {
                rsa.Import(jsonWebKey.Content);
                return rsa.VerifyData(plainBytes, signature, Alg, RSASignaturePadding.Pss);
            }
        }

        public abstract HashAlgorithmName Alg { get; }
        public abstract string AlgName { get; }
    }
}
