// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Encoders;

public class X25519KeyAgreementKey2019Standard : IVerificationMethodStandard
{
    public const string JSON_LD_CONTEXT = "http://w3id.org/security/suites/x25519-2019/v1";
    public const string TYPE = "X25519KeyAgreementKey2019";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public SignatureKeyEncodingTypes DefaultEncoding => SignatureKeyEncodingTypes.BASE58;

    public SignatureKeyEncodingTypes SupportedEncoding => SignatureKeyEncodingTypes.BASE58;

    public string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey)
    {

        return null;
    }
}