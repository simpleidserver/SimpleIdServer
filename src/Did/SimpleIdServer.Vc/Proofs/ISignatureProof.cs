// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Vc.Models;
using System.Security.Cryptography;

namespace SimpleIdServer.Vc.Proofs;

public interface ISignatureProof
{
    string Type { get; }
    string VerificationMethod { get; }
    string TransformationMethod { get; }
    HashAlgorithmName HashingMethod { get; }
    void ComputeProof(DataIntegrityProof proof, byte[] payload, IAsymmetricKey asymmetricKey, HashAlgorithmName alg);
    byte[] GetSignature(DataIntegrityProof proof);
}