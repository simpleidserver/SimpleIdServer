// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Enc = System.Text.Encoding;

namespace SimpleIdServer.Did.Crypto;

public abstract class BaseESSignatureKey : ISignatureKey
{
    private ECDsa _key;

    protected BaseESSignatureKey() { }

    protected BaseESSignatureKey(ECDsa key)
    {
        _key = key;
    }

    public abstract string Name { get; }
    protected abstract string CurveName { get; }

    public byte[] PrivateKey 
        =>  _key.ExportECPrivateKey();

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        ECPoint? q = null;
        if (publicKey != null)
            q = new ECPoint
            {
                X = publicKey.Skip(1).Take(32).ToArray(),
                Y = publicKey.Skip(33).ToArray()
            };

        _key = ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q = q.GetValueOrDefault(),
            D = privateKey
        });
    }

    public void Import(JsonWebKey jwk)
    {
        throw new NotImplementedException();
    }

    public bool Check(string content, string signature) => Check(System.Text.Encoding.UTF8.GetBytes(content), Base64UrlEncoder.DecodeBytes(signature));

    public bool Check(byte[] content, byte[] signature)
    {
        throw new NotImplementedException();
    }

    public string Sign(string content)
        => Sign(Enc.UTF8.GetBytes(content));

    public string Sign(byte[] content)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        var sig = _key.SignHash(Hash(content));
        var signature = new ECDSASignature(sig);
        var result = new List<byte>();
        result.AddRange(signature.R.ToByteArrayUnsigned());
        result.AddRange(signature.S.ToByteArrayUnsigned());
        return Base64UrlEncoder.Encode(result.ToArray());
    }

    public byte[] GetPublicKey(bool compressed = false)
        => _key.ExportSubjectPublicKeyInfo();

    public JsonWebKey GetPublicJwk()
    {
        var parameters = _key.ExportExplicitParameters(false);
        var result = new JsonWebKey
        {
            Kty = "EC",
            Crv = CurveName,
            X = Base64UrlEncoder.Encode(parameters.Q.X),
            Y = Base64UrlEncoder.Encode(parameters.Q.Y)
        };
        return result;
    }


    private static byte[] Hash(byte[] payload)
    {
        byte[] result = null;
        using (var sha256 = SHA256.Create())
            result = sha256.ComputeHash(payload);

        return result;
    }
}
