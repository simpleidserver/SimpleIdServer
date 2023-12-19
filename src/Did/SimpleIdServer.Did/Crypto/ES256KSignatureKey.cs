// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using Enc = System.Text.Encoding;

namespace SimpleIdServer.Did.Crypto;

/// <summary>
/// Documentation : https://datatracker.ietf.org/doc/html/rfc8812#section-3.2
/// </summary>
public class ES256KSignatureKey : ISignatureKey
{
    private const string _curveName = "secp256k1";
    private ECPublicKeyParameters _publicKeyParameters;
    private ECPrivateKeyParameters _privateKeyParameters;
    
    private ES256KSignatureKey() { }

    private ES256KSignatureKey(ECPublicKeyParameters publicKeyParameters)
    {
        _publicKeyParameters = publicKeyParameters;
    }

    private ES256KSignatureKey(
        ECPublicKeyParameters publicKeyParameters, 
        ECPrivateKeyParameters privateKeyParameters) : this(publicKeyParameters)
    {
        _privateKeyParameters = privateKeyParameters;
    }

    public string Name => Constants.SupportedSignatureKeyAlgs.ES256K;

    public byte[] PrivateKey => throw new System.NotImplementedException();

    public static ES256KSignatureKey Generate()
    {
        var curve = ECNamedCurveTable.GetByName(_curveName);
        var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
        var secureRandom = new SecureRandom();
        var keyParams = new ECKeyGenerationParameters(domainParams, secureRandom);
        var generator = new ECKeyPairGenerator("ECDSA");
        generator.Init(keyParams);
        var keyPair = generator.GenerateKeyPair();
        var privateKey = keyPair.Private as ECPrivateKeyParameters;
        var publicKey = keyPair.Public as ECPublicKeyParameters;
        return new ES256KSignatureKey(publicKey, privateKey);
    }

    public static ES256KSignatureKey New()
        => new ES256KSignatureKey();

    public static ES256KSignatureKey From(byte[] publicKey, byte[] privateKey)
    {
        var result = new ES256KSignatureKey();
        result.Import(publicKey, privateKey);
        return result;
    }

    public static ES256KSignatureKey From(JsonWebKey jwk)
    {
        var result = new ES256KSignatureKey();
        result.Import(jwk);
        return result;
    }

    public void Import(byte[] publicKey, byte[] privateKey)
    {
        var curve = SecNamedCurves.GetByName(_curveName);
        var domainParameters = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
        if (publicKey != null)
        {
            var q = curve.Curve.DecodePoint(publicKey);
            _publicKeyParameters = new ECPublicKeyParameters(q, domainParameters);
        }

        if(privateKey != null)
        {
            _privateKeyParameters = new ECPrivateKeyParameters(
                new Org.BouncyCastle.Math.BigInteger(1, privateKey),
                domainParameters);
        }
    }

    public void ImportPrivateKey(byte[] privateKey)
    {
    }

    public void Import(JsonWebKey jwk)
    {
        if (jwk == null) throw new ArgumentNullException(nameof(jwk));
        if (jwk.X == null || jwk.Y == null) throw new InvalidOperationException("There is no public key");
        var x = Base64UrlEncoder.DecodeBytes(jwk.X);
        var y = Base64UrlEncoder.DecodeBytes(jwk.Y);
        var curve = SecNamedCurves.GetByName(_curveName);
        var ecPoint = curve.Curve.CreatePoint(
            new Org.BouncyCastle.Math.BigInteger(1, x), 
            new Org.BouncyCastle.Math.BigInteger(1, y));
        _publicKeyParameters = new ECPublicKeyParameters(
            ecPoint, 
            new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed()));
    }

    public bool Check(string content, string signature)
    {
        throw new System.NotImplementedException();
    }

    public bool Check(byte[] content, byte[] signature)
    {
        throw new System.NotImplementedException();
    }

    public string Sign(string content)
        => Sign(Enc.UTF8.GetBytes(content));

    public string Sign(byte[] content)
    {
        // Use deterministic ECDSA https://datatracker.ietf.org/doc/html/rfc6979
        if (content == null) throw new ArgumentNullException(nameof(content));
        byte[] messageHash = Hash(content);
        ECDsaSigner signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
        signer.Init(true, _privateKeyParameters);
        var signature = signer.GenerateSignature(messageHash);
        var lst = new List<byte>();
        lst.AddRange(signature[0].ToByteArrayUnsigned());
        lst.AddRange(signature[1].ToByteArrayUnsigned());
        return Base64UrlEncoder.Encode(lst.ToArray());

    }

    public byte[] GetPublicKey(bool compressed = false)
        => _publicKeyParameters.Q.GetEncoded();

    public JsonWebKey GetPublicJwk()
    {
        var result = new JsonWebKey
        {
            Kty = "EC",
            Crv = _curveName,
            X = Base64UrlEncoder.Encode(_publicKeyParameters.Q.XCoord.GetEncoded()),
            Y = Base64UrlEncoder.Encode(_publicKeyParameters.Q.YCoord.GetEncoded())
        };
        return result;
    }

    private static byte[] Hash(byte[] payload)
    {
        var h13 = new Sha256Digest();
        h13.BlockUpdate(payload, 0, payload.Length);
        var messageHash = new byte[h13.GetDigestSize()];
        h13.DoFinal(messageHash, 0);
        return messageHash;
        /*
        byte[] result = null;
        using (var sha256 = SHA256.Create())
            result = sha256.ComputeHash(payload);

        return result;
        */
    }
}
