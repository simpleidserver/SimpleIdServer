// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoding;
using System;
using System.Linq;

namespace SimpleIdServer.Did.Key
{
    public class SignatureKeyFactory
    {
        public static ISignatureKey Build(string multicodecValue)
        {
            multicodecValue= multicodecValue.TrimStart('z');
            var payload = Base58Encoding.Decode(multicodecValue.TrimStart('z')).Skip(2).ToArray();
            switch (multicodecValue)
            {
                case string s when s.StartsWith("Q3s"):
                    {
                        // https://github.com/transmute-industries/verifiable-data/blob/e815d0c578dc33b4d135650555047eb0cf963e37/packages/secp256k1-key-pair/src/Secp256k1KeyPair.ts
                        if (payload.Length != 33) throw new InvalidOperationException("the key size is invalid, it must be equals to 33");
                        return new ES256KSignatureKey(payload);
                    }
                case string s when s.StartsWith("6Mk"):
                    {
                        if (payload.Length != 32) throw new InvalidOperationException("the key size is invalid, it must be equals to 32");
                        return new Ed25519SignatureKey(payload, null);
                    }
            }

            throw new InvalidOperationException("signature algorithm cannot be extracted");
        }
    }
}