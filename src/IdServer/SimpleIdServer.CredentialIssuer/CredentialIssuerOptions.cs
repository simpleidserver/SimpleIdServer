// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer
{
    public class CredentialIssuerOptions
    {
        public string AuthorizationServer { get; set; }
        public bool UseRealm { get; set; } = true;

        public ICollection<CredentialIssuerDisplayResult> CredentialIssuerDisplays { get; set; } = new List<CredentialIssuerDisplayResult>
        {
            new CredentialIssuerDisplayResult { Name = "SimpleIdServer" }
        };
    }
}
