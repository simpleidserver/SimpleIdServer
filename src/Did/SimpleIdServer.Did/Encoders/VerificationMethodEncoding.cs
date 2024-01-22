// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Encoders;

public interface IVerificationMethodEncoding
{
    DidDocumentVerificationMethod Encode(string type, string controller, IAsymmetricKey key, SignatureKeyEncodingTypes? encoding = null, bool includePrivateKey = false);
    IEnumerable<IVerificationMethodStandard> Standards { get; }
}

public class VerificationMethodEncoding : IVerificationMethodEncoding
{
    private readonly IEnumerable<IVerificationMethodStandard> _standards;
    private readonly IMulticodecSerializer _multicodecSerializer;

    public VerificationMethodEncoding(
        IEnumerable<IVerificationMethodStandard> standards,
        IMulticodecSerializer multicodecSerializer)
    {
        _standards = standards;
        _multicodecSerializer = multicodecSerializer;
    }

    public IEnumerable<IVerificationMethodStandard> Standards => _standards;

    public DidDocumentVerificationMethod Encode(string type,
        string controller,
        IAsymmetricKey key, 
        SignatureKeyEncodingTypes? encoding = null, 
        bool includePrivateKey = false)
    {
        var standard = _standards.Single(s => s.Type == type);
        if (encoding == null) encoding = standard.DefaultEncoding;
        if (!standard.SupportedEncoding.HasFlag(encoding)) 
            throw new InvalidOperationException("This type of encoding is not supported");
        byte[] privateKey = null;
        if(includePrivateKey)
        {
            privateKey = key.GetPrivateKey();
            if (privateKey == null) throw new InvalidOperationException("There is no private key");
        }

        var result = new DidDocumentVerificationMethod
        {
            Type = standard.Type,
            Controller = controller
        };
        switch (encoding)
        {
            case SignatureKeyEncodingTypes.BASE58:
                result.PublicKeyBase58 = Encoding.Base58Encoding.Encode(key.GetPublicKey());
                if(includePrivateKey)
                {
                    result.PrivateKeyBase58 = Encoding.Base58Encoding.Encode(privateKey);
                }

                break;
            case SignatureKeyEncodingTypes.HEX:
                result.PublicKeyHex = key.GetPublicKey(true).ToHex();
                if (includePrivateKey)
                {
                    result.PrivateKeyHex = key.GetPrivateKey().ToHex();
                }

                break;
            case SignatureKeyEncodingTypes.MULTIBASE:
                result.PublicKeyMultibase = _multicodecSerializer.SerializePublicKey(key);
                if (includePrivateKey)
                    result.SecretKeyMultibase = _multicodecSerializer.SerializePrivateKey(key);
                break;
            case SignatureKeyEncodingTypes.JWK:
                result.PublicKeyJwk = key.GetPublicJwk();
                if(includePrivateKey)
                    result.PrivateKeyJwk = key.GetPrivateJwk();
                break;
        }

        var builtId = standard.BuildId(result, key);
        if(!string.IsNullOrWhiteSpace(builtId))
        {
            result.Id = $"{controller}#{builtId}";
        }

        return result;
    }
}
