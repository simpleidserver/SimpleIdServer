// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto.Multicodec
{
    public class Ec256VerificationMethod : IVerificationMethod
    {
        public string MulticodecPublicKeyHexValue => null;

        public string MulticodecPrivateKeyHexValue => null;

        public int KeySize => 0;

        public string Kty => throw new System.NotImplementedException();

        public string CrvOrSize => throw new System.NotImplementedException();

        public IAsymmetricKey Build(byte[] publicKey, byte[] privateKey)
        {
            throw new System.NotImplementedException();
        }

        public IAsymmetricKey Build(JsonWebKey publicJwk, JsonWebKey privateJwk)
        {
            throw new System.NotImplementedException();
        }
    }
}
