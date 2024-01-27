// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Did.Models;

namespace SimpleIdServer.IdServer.CredentialIssuer;

public class CredentialIssuerOptions
{
    /// <summary>
    /// Default c_nonce expiration time in seconds.
    /// </summary>
    public double? DefaultCNonceExpirationTimeInSeconds { get; set; } = 600;
    /// <summary>
    /// Distributed Identity Document used to sign verifiable credential.
    /// </summary>
    public DidDocument DidDocument { get; set; }
    /// <summary>
    /// Identifier of the verification method used to sign the verifiable credential.
    /// </summary>
    public string VerificationMethodId { get; set; }
}
