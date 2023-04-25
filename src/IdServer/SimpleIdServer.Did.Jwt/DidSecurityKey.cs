// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.Did.Jwt
{
    public class DidSecurityKey : AsymmetricSecurityKey
    {
        private readonly ISignatureKey _signatureKey;

        public DidSecurityKey(ISignatureKey signatureKey)
        {
            _signatureKey = signatureKey;
            CryptoProviderFactory.CustomCryptoProvider = new DidCryptoProvider(_signatureKey);
        }

        public override bool HasPrivateKey => throw new System.NotImplementedException();

        public override PrivateKeyStatus PrivateKeyStatus => throw new System.NotImplementedException();

        public override int KeySize => throw new System.NotImplementedException();
    }
}
