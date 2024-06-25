// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class DeferredCredential
{
    public string TransactionId { get; set; } = null!;
    public string? CredentialId { get; set; } = null;
    public string? CredentialConfigurationId { get; set; } = null;
    public DeferredCredentialStatus Status { get; set; }
    public List<DeferredCredentialClaim> Claims { get; set; }
}

public enum DeferredCredentialStatus
{
    PENDING = 0,
    ISSUED = 1
}