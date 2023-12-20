// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;

namespace SimpleIdServer.Did.Crypto;

public interface IAsymmetricKey
{
    string Kty { get; }
    string CrvOrSize { get; }
    byte[] PrivateKey { get; }
    void Import(byte[] publicKey, byte[] privateKey);
    void Import(JsonWebKey publicKey);
    byte[] GetPublicKey(bool compressed = false);
    JsonWebKey GetPublicJwk();
}
