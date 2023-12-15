// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto
{
    public abstract class BaseESSignatureKey : ISignatureKey
    {
        private readonly ECDsa _key;

        protected BaseESSignatureKey(ECDsa key)
        {
            _key = key;
        }

        public abstract string Name { get; }
        protected abstract string CurveName { get; }

        public byte[] PrivateKey 
            =>  _key.ExportECPrivateKey();

        public byte[] GetPublicKey(bool compressed = false)
            => _key.ExportSubjectPublicKeyInfo();

        public JsonWebKey GetPublicKeyJwk()
        {
            var parameters = _key.ExportExplicitParameters(false);
            var result = new JsonWebKey();
            result.Kty = "EC";
            result.Crv = CurveName;
            result.X = Base64UrlEncoder.Encode(parameters.Q.X);
            result.Y = Base64UrlEncoder.Encode(parameters.Q.Y);
            return result;
        }

        public bool Check(string content, string signature) => Check(System.Text.Encoding.UTF8.GetBytes(content), Base64UrlEncoder.DecodeBytes(signature));

        public bool Check(byte[] content, byte[] signature)
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
    }
}
