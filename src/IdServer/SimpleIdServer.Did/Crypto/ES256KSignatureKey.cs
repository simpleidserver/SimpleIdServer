// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Crypto
{
    public class ES256KSignatureKey : BaseESSignatureKey
    {
        public ES256KSignatureKey(byte[] publicKey) : base(publicKey)
        {
        }

        public ES256KSignatureKey(byte[] publicKey, byte[] privateKey) : base(publicKey, privateKey)
        {
        }

        public override string CurveName => "secp256k1";
    }
}
