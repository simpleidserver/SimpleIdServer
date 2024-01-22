// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Encoders;

/// <summary>
/// Documentation : https://www.w3.org/TR/did-spec-registries/#verification-method-types
/// </summary>
public interface IVerificationMethodStandard
{
    string JSONLDContext { get; }
    string Type { get; }
    SignatureKeyEncodingTypes DefaultEncoding { get; }
    SignatureKeyEncodingTypes SupportedEncoding { get; }
    string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey);
}
