// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialTemplates;

public class AddCredentialTemplateDisplayRequest
{
    [JsonPropertyName(CredentialTemplateDisplayNames.Name)]
    public string Name { get; set; } = null!;
    [JsonPropertyName(CredentialTemplateDisplayNames.Locale)]
    public string Locale { get; set; } = null!;
    [JsonPropertyName(CredentialTemplateDisplayNames.Description)]
    public string? Description { get; set; } = null;
    [JsonPropertyName(CredentialTemplateDisplayNames.Logo)]
    public string? LogoUrl { get; set; } = null;
    [JsonPropertyName(CredentialTemplateDisplayNames.AltText)]
    public string? LogoAltText { get; set; } = null;
    [JsonPropertyName(CredentialTemplateDisplayNames.BackgroundColor)]
    public string? BackgroundColor { get; set; } = null;
    [JsonPropertyName(CredentialTemplateDisplayNames.TextColor)]
    public string? TextColor { get; set; } = null;
}
