// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;

namespace SimpleIdServer.Did.Crypto;

/// <summary>
///  About agreement. https://github.com/bcgit/bc-csharp/blob/e9cfe0211e25f9fc300329963050e96ef6634b08/crypto/test/src/crypto/test/X25519Test.cs#L42
/// </summary>
public class X25519AgreementKey : IAgreementKey
{
    private X25519PublicKeyParameters _publicKey;
    private X25519PrivateKeyParameters _privateKey;

    public string Kty => Constants.StandardKty.OKP;

    public string CrvOrSize => Constants.StandardCrvOrSize.X25519;

    public byte[] PrivateKey
    {
        get
        {
            if (_privateKey == null) return null;
            return _privateKey.GetEncoded();
        }
    }

    public static X25519AgreementKey From(byte[] publicKey, byte[] privateKey)
    {
        var result = new X25519AgreementKey();
        result.Import(publicKey, privateKey);
        return result;
    }

    public static X25519AgreementKey From(JsonWebKey jwk)
    {
        var result = new X25519AgreementKey();
        result.Import(jwk);
        return result;
    }

    public static X25519AgreementKey Generate()
    {
        var result = new X25519AgreementKey();
        result.Random();
        return result;
    }

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        if(publicKey != null)
        {
            _publicKey = new X25519PublicKeyParameters(publicKey);
        }

        if (privateKey != null)
        {
            _privateKey = new X25519PrivateKeyParameters(privateKey);
        }
    }

    public void Import(JsonWebKey jwk)
    {
        if(jwk == null) throw new ArgumentNullException(nameof(jwk));
        if (jwk.X == null) throw new ArgumentNullException("there is no public key");
        var payload = Base64UrlEncoder.DecodeBytes(jwk.X);
        _publicKey = new X25519PublicKeyParameters(payload);
    }

    public void Random()
    {
        var random = new SecureRandom();
        var keyGenerator = new X25519KeyPairGenerator();
        keyGenerator.Init(new X25519KeyGenerationParameters(random));
        var kv = keyGenerator.GenerateKeyPair();
        _publicKey = kv.Public as X25519PublicKeyParameters;
        _privateKey = kv.Private as X25519PrivateKeyParameters;
    }

    public byte[] GetPublicKey(bool compressed = false)
        => _publicKey.GetEncoded();

    public JsonWebKey GetPublicJwk()
    {
        var result = new JsonWebKey
        {
            Kty = Constants.StandardKty.OKP,
            Crv = CrvOrSize,
            X = Base64UrlEncoder.Encode(_publicKey.GetEncoded())
        };
        return result;
    }
}
