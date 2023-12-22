// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Builders;

namespace SimpleIdServer.Vc.Proofs;

public class Ed25519Signature2020Proof
{
    public string Type => "Ed25519Signature2020";

    /// <summary>
    /// https://www.w3.org/TR/vc-di-eddsa/#transformation-ed25519signature2020
    /// </summary>
    public string CanonizeMethod { get; }

    /// <summary>
    /// https://www.w3.org/TR/vc-di-eddsa/#hashing-ed25519signature2020
    /// </summary>
    public string Hash { get; }

    public string VerificationMethodType => Ed25519VerificationKey2020Formatter.TYPE;
}