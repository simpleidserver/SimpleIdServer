// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;

namespace SimpleIdServer.Did.Crypto
{
    public class RSASignatureKey : IAsymmetricKey
    {
        public string Kty => Constants.StandardKty.RSA;

        public string CrvOrSize => Constants.StandardCrvOrSize.RSA2048;

        public bool Check(string content, string signature)
        {
            throw new NotImplementedException();
        }

        public bool Check(byte[] content, byte[] signature)
        {
            throw new NotImplementedException();
        }

        public JsonWebKey GetPublicJwk()
        {
            throw new NotImplementedException();
        }

        public JsonWebKey GetPrivateJwk()
        {
            throw new NotImplementedException();
        }

        public byte[] GetPublicKey(bool compressed = false)
        {
            throw new NotImplementedException();
        }

        public byte[] GetPrivateKey()
        {
            throw new NotImplementedException();
        }

        public void Import(byte[] publicKey, byte[] privateKey)
        {
            throw new NotImplementedException();
        }

        public void Import(JsonWebKey publicJwk, JsonWebKey privateJwk)
        {
            throw new NotImplementedException();
        }

        public string Sign(string content)
        {
            throw new NotImplementedException();
        }

        public string Sign(byte[] content)
        {
            throw new NotImplementedException();
        }

        byte[] IAsymmetricKey.Sign(byte[] content)
        {
            throw new NotImplementedException();
        }
    }
}
