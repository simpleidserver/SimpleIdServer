// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SimpleIdServer.Vc.Models
{
    public class W3CCredentialTemplate : BaseCredentialTemplate
    {
        public W3CCredentialTemplate() { }

        public W3CCredentialTemplate(BaseCredentialTemplate credentialTemplate)
        {
            TechnicalId = credentialTemplate.TechnicalId;
            Id = credentialTemplate.Id;
            Format = credentialTemplate.Format;
            CreateDateTime = credentialTemplate.CreateDateTime;
            UpdateDateTime = credentialTemplate.UpdateDateTime;
            DisplayLst = credentialTemplate.DisplayLst;
            Parameters = credentialTemplate.Parameters;
        }

        public void AddType(string type)
        {
            Parameters.Add(new CredentialTemplateParameter
            {
                Id = Guid.NewGuid().ToString(),
                Name = W3CCredentialTemplateNames.Type,
                Value = type,
                IsArray = true,
                ParameterType = CredentialTemplateParameterTypes.STRING
            });
        }

        /// <summary>
        /// REQUIRED. JSON array designating the types a certain credential type supports.
        /// </summary>
        public IEnumerable<string> GetTypes() => Parameters.Where(p => p.Name == W3CCredentialTemplateNames.Type).Select(t => t.Value);

        public void AddCredentialSubject(string claimName, W3CCredentialSubject subject)
        {
            var dic = new Dictionary<string, object>
            {
                { claimName, subject }
            };
            Parameters.Add(new CredentialTemplateParameter
            {
                Id = Guid.NewGuid().ToString(),
                Name = W3CCredentialTemplateNames.CredentialSubjet,
                Value = JsonSerializer.Serialize(dic),
                IsArray = true,
                ParameterType = CredentialTemplateParameterTypes.JSON
            });
        }

        /// <summary>
        /// OPTIONAL. A JSON object containing a list of key value pairs, where the key identifies the claim offered in the Credential.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<W3CCredentialSubject> GetSubjects() => Parameters.Where(p => p.Name == W3CCredentialTemplateNames.CredentialSubjet).Select(t => JsonSerializer.Deserialize<W3CCredentialSubject>(t.Value));
    }
}
 