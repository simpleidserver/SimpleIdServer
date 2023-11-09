// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.CredentialIssuer
{
    public static class Constants
    {
        public static class EndPoints
        {
            public const string CredentialIssuer = ".well-known/openid-credential-issuer";
            public const string Credential = "credential";
            public const string CredentialOffer = "credential_offer";
            public const string CredentialTemplates = "credential_templates";
        }

        public static class StandardAuthorizationDetails
        {
            public const string OpenIdCredential = "openid_credential";
        }

        public static class StandardProofTypes
        {
            public const string Jwt = "jwt";
        }

        public static class StandardScopes
        {
            public static Scope CredentialTemplates = new Scope
            {
                Id = Guid.NewGuid().ToString(),
                Type = ScopeTypes.APIRESOURCE,
                Name = "credential_templates",
                Realms = new List<Domains.Realm>
                {
                    StandardRealms.Master
                },
                Protocol = ScopeProtocols.OAUTH,
                IsExposedInConfigurationEdp = true,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
        }
    }
}
