// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Models;
using System;

namespace SimpleIdServer.Did.Key;

/// <summary>
/// Documentation : https://w3c-ccg.github.io/did-method-key/
/// </summary>
public class DidKeyResolver : IDidResolver
{
    private readonly IMulticodecSerializer _serializer;
    private readonly DidKeyOptions _options;

    public DidKeyResolver(
        IMulticodecSerializer serializer, 
        DidKeyOptions options)
    {
        _serializer = serializer;
        _options = options;
    }

    public static DidKeyResolver New(DidKeyOptions options = null)
    {
        options = options ?? new DidKeyOptions();
        var serializer = MulticodecSerializerFactory.Build();
        return new DidKeyResolver(serializer, options);
    }

    public string Method => Constants.Type;

    public DidDocument Resolve(string did)
    {
        var decentralizedIdentifier = DidExtractor.Extract(did);
        if (decentralizedIdentifier.Method != Method) throw new ArgumentException($"method must be equals to {Method}");
        var multibaseValue = decentralizedIdentifier.Identifier;
        var verificationMethod = _serializer.Deserialize(multibaseValue, null);
        var builder = DidDocumentBuilder.New(did);
        var verificationMethodId = $"{did}#{multibaseValue}";
        switch(_options.PublicKeyFormat)
        {
            case Ed25519VerificationKey2020Formatter.TYPE:
                builder.AddEd25519VerificationKey2020VerificationMethod(
                    verificationMethod,
                    did,
                    VerificationMethodUsages.AUTHENTICATION | VerificationMethodUsages.ASSERTION_METHOD |
                    VerificationMethodUsages.CAPABILITY_INVOCATION | VerificationMethodUsages.CAPABILITY_DELEGATION,
                    id: verificationMethodId);
                    break;
            case JsonWebKey2020Formatter.TYPE:
                builder.AddJsonWebKeyVerificationMethod(
                    verificationMethod,
                    did,
                    VerificationMethodUsages.AUTHENTICATION | VerificationMethodUsages.ASSERTION_METHOD |
                    VerificationMethodUsages.CAPABILITY_INVOCATION | VerificationMethodUsages.CAPABILITY_DELEGATION,
                    id: verificationMethodId);
                break;
            default:
                throw new InvalidOperationException($"The key format {_options.PublicKeyFormat} is not supported");
        }

        if(_options.EnableEncryptionKeyDerivation)
        {
            throw new NotSupportedException("This feature is not yet supported");
            /*
            var publicKey = verificationMethod.GetPublicKey();
            var method = new X25519VerificationMethod();
            var key = method.Build(publicKey, null);
            builder.AddX25519KeyAgreementVerificationMethod(key,
                did,
                isReference: false);
            */
        }

        return builder.Build();
    }
}