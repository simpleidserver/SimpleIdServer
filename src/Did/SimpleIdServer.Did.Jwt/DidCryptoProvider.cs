// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;
using System;

namespace SimpleIdServer.Did.Jwt
{
    public class DidCryptoProvider : ICryptoProvider
    {
        private readonly ISignatureKey _signatureKey;

        public DidCryptoProvider(ISignatureKey signatureKey)
        {
            _signatureKey = signatureKey;
        }

        public object Create(string algorithm, params object[] args)
        {
            if (args[0] is DidSecurityKey key) return new DidSignatureProvider(key, _signatureKey.Name, _signatureKey);
            throw new NotSupportedException();
        }

        public bool IsSupportedAlgorithm(string algorithm, params object[] args) => true;

        public void Release(object cryptoInstance) { }
    }
}
