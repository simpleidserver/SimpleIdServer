// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Encoding;
using SimpleIdServer.Did.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Crypto.Multicodec;

public interface IMulticodecSerializer
{
    IAsymmetricKey Deserialize(string publicKey, string privateKey);
    IAsymmetricKey Deserialize(byte[] publicKeyPayload, byte[] privateKeyPayload);
    string SerializePublicKey(IAsymmetricKey signatureKey);
    string SerializePrivateKey(IAsymmetricKey signatureKey);
}

/// <summary>
/// Documentation : https://w3c-ccg.github.io/did-method-key/#signature-method-creation-algorithm
/// For the multicodec values : https://github.com/multiformats/multicodec/blob/master/table.csv
/// </summary>
public class MulticodecSerializer : IMulticodecSerializer
{
    private readonly IEnumerable<IVerificationMethod> _verificationMethods;

    public MulticodecSerializer(IEnumerable<IVerificationMethod> verificationMethods)
    {
        _verificationMethods = verificationMethods;
    }

    public IAsymmetricKey Deserialize(string publicKey, string privateKey)
    {
        var publicKeyPayload = MultibaseEncoding.Decode(publicKey);
        var privateKeyPayload = MultibaseEncoding.Decode(privateKey);
        return Deserialize(publicKeyPayload, privateKeyPayload);
    }

    public IAsymmetricKey Deserialize(byte[] publicKeyPayload, byte[] privateKeyPayload)
    {
        if (publicKeyPayload == null)throw new ArgumentNullException(nameof(publicKeyPayload));
        var hex = publicKeyPayload.ToHex(true);
        var verificationMethod = _verificationMethods.SingleOrDefault(m => hex.StartsWith(m.MulticodecPublicKeyHexValue));
        if (verificationMethod == null) throw new InvalidOperationException("Public key; Either the multicodec is invalid or the verification method is not supported");
        byte[] privateKey = null;
        if(privateKeyPayload != null)
        {
            var hexPrivateKey = privateKeyPayload.ToHex(true);
            if(!hexPrivateKey.StartsWith(verificationMethod.MulticodecPrivateKeyHexValue)) throw new InvalidOperationException("Private key; Either the multicodec is invalid or the verification method is not supported");
            var privateKeyMulticodecHeaderPayload = verificationMethod.MulticodecPrivateKeyHexValue.HexToByteArray();
            privateKey = privateKeyPayload.Skip(privateKeyMulticodecHeaderPayload.Length).ToArray();
        }

        var multicodecHeaderPayload = verificationMethod.MulticodecPublicKeyHexValue.HexToByteArray();
        var publicKey = publicKeyPayload.Skip(multicodecHeaderPayload.Length).ToArray();
        return verificationMethod.Build(publicKey, privateKey);
    }

    public string SerializePublicKey(IAsymmetricKey signatureKey)
    {
        var verificationMethod = _verificationMethods.SingleOrDefault(m => m.Kty == signatureKey.Kty && m.CrvOrSize == signatureKey.CrvOrSize);
        if (verificationMethod == null && signatureKey.GetPublicJwk() != null)
            verificationMethod = _verificationMethods.First(v => v.GetType() == typeof(JwkJcsPubVerificationMethod));

        var publicKey = verificationMethod.MulticodecPublicKeyHexValue.HexToByteArray().ToList();
        publicKey.AddRange(signatureKey.GetPublicKey());
        return MultibaseEncoding.Encode(publicKey.ToArray());
    }

    public string SerializePrivateKey(IAsymmetricKey signatureKey)
    {
        var verificationMethod = _verificationMethods.SingleOrDefault(m => m.Kty == signatureKey.Kty && m.CrvOrSize == signatureKey.CrvOrSize);
        if (verificationMethod == null && signatureKey.GetPrivateJwk() != null)
            verificationMethod = _verificationMethods.First(v => v.GetType() == typeof(JwkJcsPubVerificationMethod));

        var privateKey = verificationMethod.MulticodecPrivateKeyHexValue.HexToByteArray().ToList();
        privateKey.AddRange(signatureKey.GetPrivateKey());
        return MultibaseEncoding.Encode(privateKey.ToArray());
    }
}