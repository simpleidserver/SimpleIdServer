// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto.SecurityKeys;

public class JsonWebKeySecurityKey : IAsymmetricKey
{
    private readonly JsonWebKey _jwk;

    public JsonWebKeySecurityKey(JsonWebKey jwk)
    {
        _jwk = jwk;
    }

    public string Kty => null;

    public string CrvOrSize => null;

    public string JwtAlg => throw new System.NotImplementedException();

    public SigningCredentials BuildSigningCredentials(string kid = null)
    {
        return new SigningCredentials(_jwk, "EC256");
    }

    public bool CheckHash(byte[] content, byte[] signature, HashAlgorithmName? alg = null)
    {
        return false;
    }

    public JsonWebKey GetPrivateJwk()
    {
        return null;
    }

    public byte[] GetPrivateKey()
    {
        return null;
    }

    public JsonWebKey GetPublicJwk()
    {
        return _jwk;
    }

    public byte[] GetPublicKey(bool compressed = false)
    {
        return null;
    }

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        return;
    }

    public void Import(JsonWebKey publicKey, JsonWebKey privateKey)
    {
        return;
    }

    public byte[] SignHash(byte[] content, HashAlgorithmName alg)
    {
        return null;
    }
}
