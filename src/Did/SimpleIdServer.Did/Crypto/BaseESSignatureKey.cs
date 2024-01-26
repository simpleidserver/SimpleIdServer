// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto;

public abstract class BaseESSignatureKey : IAsymmetricKey
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

    public void Import(JsonWebKey publicJwk, JsonWebKey privateJwk)
    {
        if (publicJwk == null) throw new ArgumentNullException(nameof(publicJwk));
        if (publicJwk.X == null || publicJwk.Y == null) throw new InvalidOperationException("There is no public key");
        var x = Base64UrlEncoder.DecodeBytes(publicJwk.X);
        var y = Base64UrlEncoder.DecodeBytes(publicJwk.Y);
        var ecPoint = _curve.Curve.CreatePoint(
            new Org.BouncyCastle.Math.BigInteger(1, x),
            new Org.BouncyCastle.Math.BigInteger(1, y));
        var domainParameters = new ECDomainParameters(_curve.Curve, _curve.G, _curve.N, _curve.H);
        _publicKey = new ECPublicKeyParameters(
            ecPoint,
            domainParameters);
        if(privateJwk != null)
        {
            var d = Base64UrlEncoder.DecodeBytes(privateJwk.D);
            _privateKey = new ECPrivateKeyParameters(
                new Org.BouncyCastle.Math.BigInteger(1, d),
                domainParameters);
        }
    }

    public bool CheckHash(byte[] content, byte[] signature, HashAlgorithmName? alg = null)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (signature == null) throw new ArgumentNullException(nameof(signature));
        if (_publicKey == null) throw new InvalidOperationException("There is no public key");
        var sig = ExtractSignature(signature);
        var signer = new ECDsaSigner();
        signer.Init(false, _publicKey);
        return signer.VerifySignature(content, sig.R, sig.S);
    }

    public byte[] SignHash(byte[] content, HashAlgorithmName alg)
    {
        if (content == null) throw new ArgumentNullException(nameof(content));
        if (_privateKey == null) throw new InvalidOperationException("There is no private key");
        var signer = new DeterministicECDSA();
        signer.SetPrivateKey(_privateKey);
        var sig = ECDSASignature.FromDER(signer.SignHash(content));
        var lst = new List<byte>();
        lst.AddRange(sig.R.ToByteArrayUnsigned());
        lst.AddRange(sig.S.ToByteArrayUnsigned());
        return lst.ToArray();
    }

    public byte[] GetPublicKey(bool compressed = false)
    {
        var q = _publicKey.Q;
        q = q.Normalize();
        var publicKey = _curve.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(compressed);
        return publicKey;
    }

    public byte[] GetPrivateKey()
    {
        var privateKey = _privateKey.D.ToByteArrayUnsigned();
        return privateKey;
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

    public JsonWebKey GetPrivateJwk()
    {
        var d = _privateKey.D;
        var result = GetPublicJwk();
        result.D = Base64UrlEncoder.Encode(d.ToByteArrayUnsigned());
        return result;
    }

    private static ECDSASignature ExtractSignature(byte[] payload)
    {
        byte? v = null;
        if (payload.Length > 64)
        {
            v = payload[64];
            if (v == 0 || v == 1)
                v = (byte)(v + 27);
        }

        var r = new byte[32];
        Array.Copy(payload, r, 32);
        var s = new byte[32];
        Array.Copy(payload, 32, s, 0, 32);
        var result = new ECDSASignature(new BigInteger(1, r), new BigInteger(1, s));
        if (v != null) result.V = new byte[] { v.Value };
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

    public SigningCredentials BuildSigningCredentials()
    {
        throw new NotImplementedException();
    }
}
