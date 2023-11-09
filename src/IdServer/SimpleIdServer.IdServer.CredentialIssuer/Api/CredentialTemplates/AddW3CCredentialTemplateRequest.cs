// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialTemplates;

public class AddW3CCredentialTemplateRequest
{
    [JsonPropertyName(CredentialTemplateNames.Name)]
    public string Name { get; set; }
    [JsonPropertyName(CredentialTemplateNames.LogoUrl)]
    public string LogoUrl { get; set; }
    [JsonPropertyName(CredentialTemplateNames.Type)]
    public string Type { get; set; }
}
