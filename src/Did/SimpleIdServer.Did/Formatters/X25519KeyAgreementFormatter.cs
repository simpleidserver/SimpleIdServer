// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Formatters;

/// <summary>
/// https://w3c.github.io/vc-data-integrity/#key-agreement
/// </summary>
public class X25519KeyAgreementFormatter : IVerificationMethodFormatter
{
    public string JSONLDContext => throw new System.NotImplementedException();

    public string Type => throw new System.NotImplementedException();

    public IAsymmetricKey Extract(DidDocumentVerificationMethod didDocumentVerificationMethod)
    {
        throw new System.NotImplementedException();
    }

    public DidDocumentVerificationMethod Format(DidDocument idDocument, IAsymmetricKey signatureKey)
    {
        throw new System.NotImplementedException();
    }
}
