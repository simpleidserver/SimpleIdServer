// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialTemplateClaim
{
    public string Name { get; set; }
    public bool? Mandatory { get; set; }    
    public string? ValueType { get; set; }
    public List<CredentialTemplateTranslation> Translations { get; set; }
}