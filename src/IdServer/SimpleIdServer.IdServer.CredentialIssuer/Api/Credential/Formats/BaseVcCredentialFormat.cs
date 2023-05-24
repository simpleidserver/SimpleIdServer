// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc;
using SimpleIdServer.Vc.Models;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential.Formats
{
    public class BaseVcCredentialFormat
    {
        protected VerifiableCredential BuildVC(CredentialFormatParameter parameter)
        {
            var w3cTemplate = new W3CCredentialTemplate(parameter.CredentialTemplate);
            var types = w3cTemplate.GetTypes();
            types = types.Where(t => t != SimpleIdServer.Vc.Constants.DefaultVerifiableCredentialType);
            var result = VerifiableCredentialBuilder.New()
                .SetCredentialSubject(JsonObject.Parse(JsonSerializer.Serialize(parameter.Claims)).AsObject())
                .SetIssuer(parameter.Issuer)
                .Build();
            foreach (var type in types) result.Type.Add(type);
            return result;
        }
    }
}
