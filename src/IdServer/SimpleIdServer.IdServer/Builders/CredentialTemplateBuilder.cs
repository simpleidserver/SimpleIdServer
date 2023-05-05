// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Builders
{
    public class CredentialTemplateBuilder
    {
        private readonly CredentialTemplate _credentialTemplate;

        private CredentialTemplateBuilder(CredentialTemplate credentialTemplate)
        {
            _credentialTemplate = credentialTemplate;
        }

        public static CredentialTemplateBuilder NewW3CCredential(string name, string logoUrl, string type, Realm realm)
        {
            realm = realm ?? Constants.StandardRealms.Master;
            var result = new CredentialTemplate
            {
                TechnicalId = Guid.NewGuid().ToString(),
                Id = $"{name}_JWT",
                Format = Vc.Constants.CredentialTemplateProfiles.W3CVerifiableCredentials,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Realms = new List<Realm> { realm },
                DisplayLst = new List<CredentialTemplateDisplay>
                {
                    new CredentialTemplateDisplay
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = name,
                        LogoUrl = logoUrl,
                        Locale = "en-US"
                    }
                }
            };
            result.Parameters.Add(new CredentialTemplateParameter
            {
                Id = Guid.NewGuid().ToString(),
                Name = "type",
                Value = type,
                JsonPath = "$.type"
            });
            return new CredentialTemplateBuilder(result);
        }

        public CredentialTemplate Build() => _credentialTemplate;
    }
}
