// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Builders;

/// <summary>
/// Documentation : https://www.w3.org/TR/did-spec-registries/#verification-method-types
/// </summary>
public interface IVerificationMethodFormatter
{
    string JSONLDContext { get; }
    string Type { get; }
    DidDocumentVerificationMethod Format(DidDocument idDocument, ISignatureKey signatureKey);
    ISignatureKey Extract(DidDocumentVerificationMethod didDocumentVerificationMethod);
}
