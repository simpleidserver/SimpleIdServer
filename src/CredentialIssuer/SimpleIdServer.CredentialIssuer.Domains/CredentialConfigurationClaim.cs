// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialConfigurationClaim
{
    public string Id {  get; set; }
    public string SourceUserClaimName { get; set; }
    public string Name { get; set; }
    public bool? Mandatory { get; set; }    
    public string? ValueType { get; set; }
    public virtual List<CredentialConfigurationTranslation> Translations { get; set; } = new List<CredentialConfigurationTranslation>();
    public virtual CredentialConfiguration CredentialConfiguration { get; set; }
}