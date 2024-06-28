// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Key;

/// <summary>
/// Documentation : https://w3c-ccg.github.io/did-method-key/
/// </summary>
public class DidKeyResolver : IDidResolver
{
    private readonly IMulticodecSerializer _serializer;
    private readonly DidKeyOptions _options;
    private readonly IEnumerable<IVerificationMethodStandard> _verificationMethodsStandardLst;

    public DidKeyResolver(
        IMulticodecSerializer serializer, 
        DidKeyOptions options,
        IEnumerable<IVerificationMethodStandard> verificationMethodsStandardLst)
    {
        _serializer = serializer;
        _options = options;
        _verificationMethodsStandardLst = verificationMethodsStandardLst;
    }

    public static DidKeyResolver New(DidKeyOptions options = null)
    {
        options = options ?? new DidKeyOptions();
        var serializer = MulticodecSerializerFactory.Build();
        return new DidKeyResolver(serializer, options, VerificationMethodStandardFactory.GetAll());
    }

    public string Method => Constants.Type;

    public Task<DidDocument> Resolve(string did, CancellationToken cancellationToken)
    {
        var decentralizedIdentifier = DidExtractor.Extract(did);
        if (decentralizedIdentifier.Method != Method) throw new ArgumentException($"method must be equals to {Method}");
        did = decentralizedIdentifier.GetDidWithoutFragment();
        var multibaseValue = decentralizedIdentifier.Identifier;
        var verificationMethod = _serializer.Deserialize(multibaseValue, null);
        var builder = DidDocumentBuilder.New(did);
        var verificationMethodId = $"{did}#{multibaseValue}";
        var publicKeyFormat = _options.PublicKeyFormat;
        if(string.IsNullOrWhiteSpace(publicKeyFormat))
        {
            if (verificationMethod.GetType() == typeof(JsonWebKeySecurityKey))
                publicKeyFormat = JsonWebKey2020Standard.TYPE;
            else
            {
                var verificationMethodStandard = _verificationMethodsStandardLst.FirstOrDefault(m => m.SupportedCurves.Contains(verificationMethod.CrvOrSize));
                if (verificationMethodStandard != null)
                    publicKeyFormat = verificationMethodStandard.Type;
                else
                    publicKeyFormat = Ed25519VerificationKey2020Standard.TYPE;
            }
        }

        builder.AddVerificationMethod(publicKeyFormat,
            verificationMethod,
            did,
            VerificationMethodUsages.AUTHENTICATION | VerificationMethodUsages.ASSERTION_METHOD |
            VerificationMethodUsages.CAPABILITY_INVOCATION | VerificationMethodUsages.CAPABILITY_DELEGATION,
            callback: (c) =>
            {
                c.Id = verificationMethodId;
            });
        if (_options.EnableEncryptionKeyDerivation)
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

        return Task.FromResult(builder.Build());
    }
}