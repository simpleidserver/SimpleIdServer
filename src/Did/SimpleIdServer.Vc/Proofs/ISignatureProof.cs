// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Vc.Models;

namespace SimpleIdServer.Vc.Proofs;

public interface ISignatureProof
{
    string Type { get; }
    string VerificationMethod { get; }
    string TransformationMethod { get; }
    string HashingMethod { get; }
    DataIntegrityProof ComputeProof(byte[] payload, IAsymmetricKey asymmetricKey);
    byte[] GetSignature(DataIntegrityProof proof);
}