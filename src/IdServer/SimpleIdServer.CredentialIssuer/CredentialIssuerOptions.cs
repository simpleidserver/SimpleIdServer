// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer
{
    public class CredentialIssuerOptions
    {
        /// <summary>
        /// If the credential issuer is different than the authorization server then set the URL.
        /// </summary>
        public string AuthorizationServer { get; set; } = null;
        /// <summary>
        /// Enable or disable realm.
        /// </summary>
        public bool UseRealm { get; set; } = false;

        public ICollection<CredentialIssuerDisplayResult> CredentialIssuerDisplays { get; set; } = new List<CredentialIssuerDisplayResult>
        {
            new CredentialIssuerDisplayResult { Name = "SimpleIdServer" }
        };
    }
}
