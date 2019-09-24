// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Jwt.Extensions;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.Jwt.Jws.Handlers
{
    public abstract class HMACSignHandler : ISignHandler
    {
        public string Sign(string payload, JsonWebKey jsonWebKey)
        {
            var bytesToSigned = ASCIIEncoding.ASCII.GetBytes(payload);
            using (var hash = HMAC)
            {
                hash.Import(jsonWebKey.Content);
                var sig = hash.ComputeHash(bytesToSigned).Base64EncodeBytes();
                return sig;
            }
        }

        public bool Verify(string payload, byte[] signature, JsonWebKey jsonWebKey)
        {
            var newSig = Sign(payload, jsonWebKey);
            var oldSig = signature.Base64EncodeBytes();
            return newSig == oldSig;
        }

        public abstract HMAC HMAC { get; }
        public abstract string AlgName { get; }
    }
}
