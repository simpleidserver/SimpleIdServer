// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Encoding;
using SimpleIdServer.Did.Models;
using System;
using System.Linq;

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
        return new DidKeyResolver(
            new MulticodecSerializer(new IVerificationMethod[] 
            { 
                new Ed25519VerificationMethod(),
                new Es256KVerificationMethod(),
                new Es256VerificationMethod(), 
                new Es384VerificationMethod(), 
                new X25519VerificationMethod()
            }),
            options);
    }

    public string Method => Constants.Type;

    public DidDocument Resolve(string did)
    {
        const char forbiddenCharacter = 'z';
        var decentralizedIdentifier = DidExtractor.Extract(did);
        if (decentralizedIdentifier.Method != Method) throw new ArgumentException($"method must be equals to {Method}");
        var multibaseValue = decentralizedIdentifier.Identifier;
        if (!multibaseValue.StartsWith(forbiddenCharacter)) throw new ArgumentException("The multiBaseValue must begin with the letter z");
        var payload = Base58Encoding.Decode(multibaseValue.TrimStart(forbiddenCharacter))
            .ToArray();
        var verificationMethod = _serializer.Deserialize(payload);
        var builder = DidDocumentBuilder.New(did);
        if (_options.IsMultibaseVerificationMethod)
            builder.AddPublicKeyMultibaseVerificationMethod(
                verificationMethod,
                did,
                VerificationMethodUsages.AUTHENTICATION | VerificationMethodUsages.ASSERTION_METHOD |
                VerificationMethodUsages.CAPABILITY_INVOCATION | VerificationMethodUsages.CAPABILITY_DELEGATION);
        return builder.Build();
    }
}