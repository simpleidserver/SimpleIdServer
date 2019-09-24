// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public abstract class ECDSASignHandler : ISignHandler
    {
        public string Sign(string payload, JsonWebKey jsonWebKey)
        {
            var bytesToBeSigned = ASCIIEncoding.ASCII.GetBytes(payload);
            using (var dsa = new ECDsaCng())
            {
                dsa.HashAlgorithm = CngHashAlg;
                dsa.Import(jsonWebKey.Content);
                return dsa.SignData(bytesToBeSigned, HashAlg).Base64EncodeBytes();
            }
        }

        public bool Verify(string payload, byte[] signature, JsonWebKey jsonWebKey)
        {
            var plainBytes = ASCIIEncoding.ASCII.GetBytes(payload);
            using (var dsa = new ECDsaCng())
            {
                dsa.HashAlgorithm = CngHashAlg;
                dsa.Import(jsonWebKey.Content);
                return dsa.VerifyData(plainBytes, signature, HashAlg);
            }
        }

        public abstract string AlgName { get; }
        public abstract CngAlgorithm CngHashAlg { get; }
        public abstract HashAlgorithmName HashAlg { get; }
    }
}