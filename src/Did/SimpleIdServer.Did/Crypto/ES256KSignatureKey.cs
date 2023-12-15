// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace SimpleIdServer.Did.Crypto;

public class ES256KSignatureKey : ISignatureKey
{
    private const string _curveName = "secp256k1";
    private readonly ECPublicKeyParameters _publicKeyParameters;
    private readonly ECPrivateKeyParameters _privateKeyParameters;

    private ES256KSignatureKey(ECPublicKeyParameters publicKeyParameters, ECPrivateKeyParameters privateKeyParameters) 
    {
        _publicKeyParameters = publicKeyParameters;
        _privateKeyParameters = privateKeyParameters;
    }

    public string Name => Constants.SupportedSignatureKeyAlgs.ES256K;

    public byte[] PrivateKey => throw new System.NotImplementedException();

    public static ES256KSignatureKey New()
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

    public bool Check(string content, string signature)
    {
        throw new System.NotImplementedException();
    }

    public bool Check(byte[] content, byte[] signature)
    {
        throw new System.NotImplementedException();
    }

    public byte[] GetPublicKey(bool compressed = false)
        => _publicKeyParameters.Q.GetEncoded();

    public JsonWebKey GetPublicKeyJwk()
    {
        var result = new JsonWebKey();
        result.Kty = "EC";
        result.Crv = _curveName;
        result.X = Base64UrlEncoder.Encode(_publicKeyParameters.Q.XCoord.GetEncoded());
        result.Y = Base64UrlEncoder.Encode(_publicKeyParameters.Q.YCoord.GetEncoded());
        return result;
    }

    public string Sign(string content)
    {
        throw new System.NotImplementedException();
    }

    public string Sign(byte[] content)
    {
        throw new System.NotImplementedException();
    }
}
