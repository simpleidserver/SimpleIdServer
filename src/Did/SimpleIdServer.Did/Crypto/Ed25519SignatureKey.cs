// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Linq;

namespace SimpleIdServer.Did.Crypto;

/// <summary>
/// Documentation : https://www.w3.org/community/reports/credentials/CG-FINAL-di-eddsa-2020-20220724/
/// </summary>
public class Ed25519SignatureKey : ISignatureKey
{
    private Ed25519PublicKeyParameters _publicKey;
    private Ed25519PrivateKeyParameters _privateKey;

    private Ed25519SignatureKey() { }

    private Ed25519SignatureKey(Ed25519PublicKeyParameters publicKey)
    {
        _publicKey = publicKey;
    }

    private Ed25519SignatureKey(Ed25519PublicKeyParameters publicKey, Ed25519PrivateKeyParameters privateKey) : this(publicKey)
    {
        _privateKey = privateKey;
    }

    public string Kty => Constants.StandardKty.OKP;

    public string CrvOrSize => Constants.StandardCrvOrSize.Ed25519;

    public byte[] PrivateKey
    {
        get
        {
            if (_privateKey == null) return null;
            return _privateKey.GetEncoded();
        }
    }

    public static Ed25519SignatureKey From(byte[] publicKey, byte[] privateKey)
    {
        var result = new Ed25519SignatureKey();
        result.Import(publicKey, privateKey);
        return result;
    }

    public static Ed25519SignatureKey From(JsonWebKey jwk)
    {
        var result = new Ed25519SignatureKey();
        result.Import(jwk);
        return result;
    }

    public static Ed25519SignatureKey Generate()
    {
        var gen = new Ed25519KeyPairGenerator();
        gen.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
        var keyPair = gen.GenerateKeyPair();
        return new Ed25519SignatureKey((Ed25519PublicKeyParameters)keyPair.Public, (Ed25519PrivateKeyParameters)keyPair.Private);
    }

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        if(publicKey != null)
        {
            if (publicKey.Length != 32) throw new InvalidOperationException("Public key must have 32 bytes");
            _publicKey = new Ed25519PublicKeyParameters(publicKey);
        }

        if(privateKey != null)
        {
            if (privateKey.Length != 64 && privateKey.Length != 32) throw new InvalidOperationException("Private key must have 64 or 32 bytes");
            _privateKey = new Ed25519PrivateKeyParameters(privateKey.Take(32).ToArray());
            if (privateKey.Length == 64) _publicKey = new Ed25519PublicKeyParameters(privateKey.Skip(32).ToArray());
        }
    }

    public void Import(JsonWebKey publicKey)
    {
        if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));
        if (publicKey.X == null) throw new InvalidOperationException("There is no public key");
        var payload = Base64UrlEncoder.DecodeBytes(publicKey.X);
        _publicKey = new Ed25519PublicKeyParameters(payload);
    }

    public bool Check(string content, string signature) 
        => Check(System.Text.Encoding.UTF8.GetBytes(content), Base64UrlEncoder.DecodeBytes(signature));

    public bool Check(byte[] payload, byte[] signaturePayload)
    {
        if (_publicKey == null) throw new InvalidOperationException("There is no public key");
        var verifier = new Ed25519Signer();
        verifier.Init(false, _publicKey);
        verifier.BlockUpdate(payload, 0, payload.Length);
        return verifier.VerifySignature(signaturePayload);
    }

    public string Sign(string content) 
        => Sign(System.Text.Encoding.UTF8.GetBytes(content));

    public string Sign(byte[] payload)
    {
        if (_privateKey == null) throw new InvalidOperationException("There is no private key");
        var signer = new Ed25519Signer();
        signer.Init(true, _privateKey);
        signer.BlockUpdate(payload, 0, payload.Length);
        return Base64UrlEncoder.Encode(signer.GenerateSignature());
    }

    public byte[] GetPublicKey(bool compressed = false)
    {
        if (_publicKey == null) return null;
        return _publicKey.GetEncoded();
    }

    public JsonWebKey GetPublicJwk()
    {
        var result = new JsonWebKey
        {
            Kty = Kty,
            Crv = CrvOrSize,
            X = Base64UrlEncoder.Encode(_publicKey.GetEncoded())
        };
        return result;
    }
}