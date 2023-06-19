// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Did.Crypto
{
    public class ES256SignatureKey : BaseESSignatureKey
    {
        public ES256SignatureKey(byte[] publicKey) : base(publicKey)
        {
        }

        public ES256SignatureKey(byte[] publicKey, byte[] privateKey) : base(publicKey, privateKey)
        {
        }

        public override string CurveName => "secp256r1";
        public override string Name => Constants.SupportedSignatureKeyAlgs.ES256;
    }
}
