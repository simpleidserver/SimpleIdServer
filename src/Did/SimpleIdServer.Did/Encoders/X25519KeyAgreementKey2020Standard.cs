// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Encoders;

/// <summary>
/// https://w3c.github.io/vc-data-integrity/#key-agreement
/// </summary>
public class X25519KeyAgreementKey2020Standard : IVerificationMethodStandard
{
    public const string JSON_LD_CONTEXT = "https://w3id.org/security/suites/x25519-2020/v1";
    public const string TYPE = "X25519KeyAgreementKey2020";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public SignatureKeyEncodingTypes DefaultEncoding => SignatureKeyEncodingTypes.MULTIBASE;

    public SignatureKeyEncodingTypes SupportedEncoding => SignatureKeyEncodingTypes.MULTIBASE;

    public string BuildId(DidDocumentVerificationMethod verificationMethod, IAsymmetricKey asymmKey)
    {

        return null;
    }

    /*
    public SignatureKeyEncodingTypes Encoding => SignatureKeyEncodingTypes.MULTIBASE;

    public IAsymmetricKey Extract(DidDocumentVerificationMethod didDocumentVerificationMethod)
        => _serializer.Deserialize(didDocumentVerificationMethod.PublicKeyMultibase, didDocumentVerificationMethod.SecretKeyMultibase);

    public DidDocumentVerificationMethod Format(DidDocument idDocument, IAsymmetricKey signatureKey, bool includePrivateKey)
    {
        var publicKey = _serializer.SerializePublicKey(signatureKey);
        var result = new DidDocumentVerificationMethod
        {
            Id = $"{idDocument.Id}#{publicKey}",
            PublicKeyMultibase = publicKey
        };
        if(includePrivateKey)
        {
            result.SecretKeyMultibase = _serializer.SerializePrivateKey(signatureKey);
        }

        return result;
    }
    */
}
