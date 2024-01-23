// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Encoders;
using SimpleIdServer.Did.Encoding;
using SimpleIdServer.Vc.Canonize;
using SimpleIdServer.Vc.Models;
using System.Security.Cryptography;

namespace SimpleIdServer.Vc.Proofs;

/// <summary>
/// https://www.w3.org/TR/vc-di-eddsa/#hashing-ed25519signature2020
/// </summary>
public class Ed25519Signature2020Proof : ISignatureProof
{
    public string Type => "Ed25519Signature2020";

    public string JsonLdContext => Ed25519VerificationKey2020Standard.JSON_LD_CONTEXT;

    public string VerificationMethod => Ed25519VerificationKey2020Standard.TYPE;

    public string TransformationMethod => RdfCanonize.NAME;

    public HashAlgorithmName HashingMethod => HashAlgorithmName.SHA256;

    public void ComputeProof(DataIntegrityProof proof, byte[] payload, IAsymmetricKey asymmetricKey, HashAlgorithmName alg)
    {
        var signature = asymmetricKey.SignHash(payload, alg);
        proof.ProofValue = MultibaseEncoding.Encode(signature);
    }

    public byte[] GetSignature(DataIntegrityProof proof)
    {
        return MultibaseEncoding.Decode(proof.ProofValue);
    }
}