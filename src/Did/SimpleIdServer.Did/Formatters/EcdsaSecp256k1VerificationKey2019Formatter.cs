// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;

namespace SimpleIdServer.Did.Formatters;

public class EcdsaSecp256k1VerificationKey2019Formatter : IVerificationMethodFormatter
{
    public static string TYPE = "EcdsaSecp256k1VerificationKey2019";
    public static string JSON_LD_CONTEXT = "https://w3id.org/security/suites/secp256k1-2019/v1";

    public string JSONLDContext => JSON_LD_CONTEXT;

    public string Type => TYPE;

    public DidDocumentVerificationMethod Format(DidDocument idDocument, IAsymmetricKey signatureKey, bool includePrivateKey)
    {
        return new DidDocumentVerificationMethod
        {
            Id = $"{idDocument.Id}#controllerKey",
            Type = TYPE,
            Controller = idDocument.Id,
            PublicKeyHex = signatureKey.GetPublicKey(true).ToHex()
        };
    }

    public IAsymmetricKey Extract(DidDocumentVerificationMethod didDocumentVerificationMethod)
    {
        var payload = didDocumentVerificationMethod.PublicKeyHex.HexToByteArray();
        return ES256KSignatureKey.From(payload, null);
    }
}
