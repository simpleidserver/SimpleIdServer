// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Security.Cryptography;

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

    public string JwtAlg => Constants.StandardJwtAlgs.X25519;

    public static X25519AgreementKey From(byte[] publicKey, byte[] privateKey)
    {
        var result = new X25519AgreementKey();
        result.Import(publicKey, privateKey);
        return result;
    }

    public static X25519AgreementKey From(JsonWebKey publicJwk, JsonWebKey privateJwk)
    {
        var result = new X25519AgreementKey();
        result.Import(publicJwk, privateJwk);
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

    public void Import(JsonWebKey publicJwk, JsonWebKey privateJwk)
    {
        if(publicJwk == null) throw new ArgumentNullException(nameof(publicJwk));
        if (publicJwk.X == null) throw new ArgumentNullException("there is no public key");
        var payload = Base64UrlEncoder.DecodeBytes(publicJwk.X);
        _publicKey = new X25519PublicKeyParameters(payload);
        if (privateJwk != null)
        {
            payload = Base64UrlEncoder.DecodeBytes(privateJwk.D);
            _privateKey = new X25519PrivateKeyParameters(payload);
        }
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

    public byte[] GetPrivateKey()
        => _privateKey.GetEncoded();

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

    public JsonWebKey GetPrivateJwk()
    {
        var result = GetPublicJwk();
        result.D = Base64UrlEncoder.Encode(_privateKey.GetEncoded());
        return result;
    }

    public byte[] Sign()
    {
        return null;
    }

    public byte[] SignHash(byte[] content, HashAlgorithmName alg)
    {
        throw new NotSupportedException("Cannot be used for signing");
    }

    public bool CheckHash(byte[] content, byte[] signature, HashAlgorithmName? alg = null)
    {
        throw new NotSupportedException("Cannot be used for signing");
    }

    public SigningCredentials BuildSigningCredentials(string kid = null)
    {
        throw new NotImplementedException();
    }
}
