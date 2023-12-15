// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto
{
    public interface ISignatureKey
    {
        string Name { get; }
        bool Check(string content, string signature);
        bool Check(byte[] content, byte[] signature);
        string Sign(string content);
        string Sign(byte[] content);
        byte[] PrivateKey { get; }
        byte[] GetPublicKey(bool compressed = false);
        JsonWebKey GetPublicKeyJwk();
    }
}