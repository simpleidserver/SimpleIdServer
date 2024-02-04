// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Domains;

public class CredentialConfigurationTranslation
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("locale")]
    public string? Locale { get; set; } = null;
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; } = null;
    [JsonPropertyName("logo_alt_text")]
    public string? LogoAltText { get; set; } = null;
    [JsonPropertyName("description")]
    public string? Description { get; set; } = null;
    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; } = null;
    [JsonPropertyName("text_color")]
    public string? TextColor { get; set; } = null;
    [JsonIgnore]
    public virtual CredentialConfigurationClaim CredentialConfigurationClaim { get; set; }
    [JsonIgnore]
    public virtual CredentialConfiguration CredentialConfiguration { get; set; }
}
