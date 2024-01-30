// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class Credential
{
    public string Id { get; set; }
    public string CredentialId { get; set; }
    public string Subject { get; set; }
    public string CredentialConfigurationId { get; set; }
    public DateTime? IssueDateTime { get; set; }
    public DateTime? ExpirationDateTime { get; set; }
    public virtual CredentialConfiguration Configuration { get; set; }
}
