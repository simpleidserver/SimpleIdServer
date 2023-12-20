// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Enc = System.Text.Encoding;

namespace SimpleIdServer.Did.Crypto;

public abstract class BaseESSignatureKey : ISignatureKey
{
    private X9ECParameters _curve;
    private ECPublicKeyParameters _publicKey;
    private ECPrivateKeyParameters _privateKey;

    protected BaseESSignatureKey(X9ECParameters curve) 
    {
        if (curve == null) throw new ArgumentNullException(nameof(curve));
        _curve = curve;
    }

    public string Kty => Constants.StandardKty.EC;

    public abstract string CrvOrSize { get; }

    public byte[] PrivateKey
    {
        get
        {
            if (_privateKey == null) return null;
            return _privateKey.D.ToByteArrayUnsigned();
        }
    }

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        var domainParameters = new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H);
        if(publicKey != null)
        {
            var q = _curve.Curve.DecodePoint(publicKey);
            _publicKey = new ECPublicKeyParameters(Kty, q, domainParameters);
        }

        if (privateKey != null)
            _privateKey = new ECPrivateKeyParameters(
                new Org.BouncyCastle.Math.BigInteger(1, privateKey),
                domainParameters);
    }

    public void Import(JsonWebKey jwk)
    {
        if (jwk == null) throw new ArgumentNullException(nameof(jwk));
        if (jwk.X == null || jwk.Y == null) throw new InvalidOperationException("There is no public key");
        var x = Base64UrlEncoder.DecodeBytes(jwk.X);
        var y = Base64UrlEncoder.DecodeBytes(jwk.Y);
        var ecPoint = _curve.Curve.CreatePoint(
            new Org.BouncyCastle.Math.BigInteger(1, x),
            new Org.BouncyCastle.Math.BigInteger(1, y));
        _publicKey = new ECPublicKeyParameters(
            ecPoint,
            new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H));
    }

    public bool Check(string content, string signature) 
        => Check(System.Text.Encoding.UTF8.GetBytes(content), Base64UrlEncoder.DecodeBytes(signature));

    public bool Check(byte[] content, byte[] signature)
    {
        throw new NotImplementedException();
    }

    public string Sign(string content)
        => Sign(Enc.UTF8.GetBytes(content));

    public string Sign(byte[] content)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (_privateKey == null) throw new InvalidOperationException("There is no private key");
        var hash = Hash(content);
        var signer = new DeterministicECDSA();
        signer.SetPrivateKey(_privateKey);
        var sig = ECDSASignature.FromDER(signer.SignHash(hash));
        var lst = new List<byte>();
        lst.AddRange(sig.R.ToByteArrayUnsigned());
        lst.AddRange(sig.S.ToByteArrayUnsigned());
        return Base64UrlEncoder.Encode(lst.ToArray());
    }

    public byte[] GetPublicKey(bool compressed = false)
    {
        var q = _publicKey.Q;
        q = q.Normalize();
        var publicKey = _curve.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(compressed);
        return publicKey;
    }

    public JsonWebKey GetPublicJwk()
    {
        var q = _publicKey.Q;
        q = q.Normalize();
        var result = new JsonWebKey
        {
            Kty = Kty,
            Crv = CrvOrSize,
            X = Base64UrlEncoder.Encode(q.XCoord.GetEncoded()),
            Y = Base64UrlEncoder.Encode(q.YCoord.GetEncoded())
        };
        return result;
    }

    protected void Random()
    {
        var random = new SecureRandom();
        var generator = new ECKeyPairGenerator();
        var parameters = new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H);
        generator.Init(new ECKeyGenerationParameters(parameters, random));
        var keyPair = generator.GenerateKeyPair();
        _publicKey = keyPair.Public as ECPublicKeyParameters;
        _privateKey = keyPair.Private as ECPrivateKeyParameters;
    }

    private static byte[] Hash(byte[] payload)
    {
        byte[] result = null;
        using (var sha256 = SHA256.Create())
            result = sha256.ComputeHash(payload);

        return result;
    }
}
