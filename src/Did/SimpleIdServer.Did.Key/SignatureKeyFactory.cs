// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Key
{
    public class SignatureKeyFactory
    {
        public static Dictionary<(string, byte[]) , string> AlgToMulticodec = new Dictionary<(string, byte[]), string>
        {
            { ("Q3s", new byte[] { 231, 1 }),  Did.Constants.SupportedSignatureKeyAlgs.ES256K },
            { ("6Mk", new byte[] { 237, 1 }), Did.Constants.SupportedSignatureKeyAlgs.Ed25519 }
        };

        public static ISignatureKey Build(string multicodecValue)
        {
            multicodecValue = multicodecValue.TrimStart('z');
            var payload = Base58Encoding.Decode(multicodecValue).Skip(2).ToArray();
            if (!AlgToMulticodec.Any(kvp => multicodecValue.StartsWith(kvp.Key.Item1))) throw new InvalidOperationException("signature algorithm cannot be extracted");
            var alg = AlgToMulticodec.First(kvp => multicodecValue.StartsWith(kvp.Key.Item1)).Value;
            switch (alg)
            {
                case Did.Constants.SupportedSignatureKeyAlgs.ES256K:
                {
                    // https://github.com/transmute-industries/verifiable-data/blob/e815d0c578dc33b4d135650555047eb0cf963e37/packages/secp256k1-key-pair/src/Secp256k1KeyPair.ts
                    if (payload.Length != 33) throw new InvalidOperationException("the key size is invalid, it must be equals to 33");
                    // return new ES256KSignatureKey(payload);
                    return null;
                }
                default:
                {
                        if (payload.Length != 32) throw new InvalidOperationException("the key size is invalid, it must be equals to 32");
                        return new Ed25519SignatureKey(payload, null);
                }
            }
        }
    }
}