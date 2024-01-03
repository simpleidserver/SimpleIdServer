// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Builders;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoding;
using SimpleIdServer.Vc.Canonize;
using SimpleIdServer.Vc.Hashing;
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.Vc.Proofs;

/// <summary>
/// https://www.w3.org/TR/vc-di-eddsa/#hashing-ed25519signature2020
/// </summary>
public class Ed25519Signature2020Proof : ISignatureProof
{
    public string Type => "Ed25519Signature2020";

    public string JsonLdContext => Ed25519VerificationKey2020Formatter.JSON_LD_CONTEXT;

    public string VerificationMethod => Ed25519VerificationKey2020Formatter.TYPE;

    public string TransformationMethod => RdfCanonize.NAME;

    public string HashingMethod => SHA256Hash.NAME;

    public DataIntegrityProof ComputeProof(byte[] payload, IAsymmetricKey asymmetricKey)
    {
        var signature = asymmetricKey.Sign(payload);
        var result = new DataIntegrityProof
        {
            ProofValue = MultibaseEncoding.Encode(signature)
        };
        return result;
    }

    public byte[] GetSignature(DataIntegrityProof proof)
    {
        return MultibaseEncoding.Decode(proof.ProofValue);
    }
}