// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto;

public class RSA2048SignatureKey : ISignatureKey
{
    private readonly RSACryptoServiceProvider _rsa;

    public RSA2048SignatureKey(RSACryptoServiceProvider rsa)
    {
        _rsa = rsa;
    }

    public string Name => throw new NotImplementedException();

    public byte[] PrivateKey
        => _rsa.ExportRSAPrivateKey();

    public static RSA2048SignatureKey New()
    {
        var rsa = new RSACryptoServiceProvider(2048);
        return new RSA2048SignatureKey(rsa);
    }

    public bool Check(string content, string signature)
    {
        throw new NotImplementedException();
    }

    public bool Check(byte[] content, byte[] signature)
    {
        throw new NotImplementedException();
    }

    public byte[] GetPublicKey(bool compressed = false)
        => _rsa.ExportRSAPublicKey();

    public JsonWebKey GetPublicKeyJwk()
    {
        var result = new JsonWebKey();
        var publicKey = _rsa.ExportParameters(false);
        result.Kty = "RSA";
        result.E = Base64UrlEncoder.Encode(publicKey.Exponent);
        result.N = Base64UrlEncoder.Encode(publicKey.Modulus);
        return result;
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
