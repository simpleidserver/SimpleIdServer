// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.CredentialIssuer
{
    public class CredentialIssuerOptions
    {
        public ICollection<CredentialIssuerDisplayResult> CredentialIssuerDisplays { get; set; } = new List<CredentialIssuerDisplayResult>
        {
            new CredentialIssuerDisplayResult { Name = "SimpleIdServer" }
        };
    }
}
