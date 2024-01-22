// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Encoders;

/// <summary>
/// https://identity.foundation/JcsEd25519Signature2020/
/// </summary>
public class Ed25519VerificationKey2018Standard: IVerificationMethodStandard
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/ed25519-2018/v1";
    public const string TYPE = "Ed25519VerificationKey2018";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public SignatureKeyEncodingTypes DefaultEncoding => SignatureKeyEncodingTypes.BASE58;

    public SignatureKeyEncodingTypes SupportedEncoding => SignatureKeyEncodingTypes.BASE58;

    public string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey)
    {

        return null;
    }
}
