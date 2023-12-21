// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto.Multicodec;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Did.Key;

public class DidKeyDocumentDeploy
{
    private readonly IEnumerable<IVerificationMethodFormatter> _formatters;
    private readonly IMulticodecSerializer _serializer;
    private readonly DidKeyOptions _options;

    public DidKeyDocumentDeploy(
        IEnumerable<IVerificationMethodFormatter> formatters,
        IMulticodecSerializer serializer,
        DidKeyOptions options)
    {
        _formatters = formatters;
        _serializer = serializer;
        _options = options;
    }

    public Task<string> Deploy(DidDocument document)
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        if (document.VerificationMethod == null || !document.VerificationMethod.Any()) 
            throw new InvalidOperationException("At least one verification method must be present");
        var formatter = _formatters.Single(f => 
            (_options.IsMultibaseVerificationMethod && f.JSONLDContext == Ed25519VerificationKey2020Formatter.JSON_LD_CONTEXT) ||
            (_options.IsMultibaseVerificationMethod == false && f.JSONLDContext == JsonWebKey2020Formatter.JSON_LD_CONTEXT));
        var publicKey = formatter.GetPublicKey(document.VerificationMethod.First());

        return null;
    }
}
