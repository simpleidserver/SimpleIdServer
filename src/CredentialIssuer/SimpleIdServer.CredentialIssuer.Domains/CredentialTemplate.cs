// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialTemplate
{
    public string Id { get; set; }
    public string JsonLdContext { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public List<CredentialTemplateClaim> Claims { get; set; }
}