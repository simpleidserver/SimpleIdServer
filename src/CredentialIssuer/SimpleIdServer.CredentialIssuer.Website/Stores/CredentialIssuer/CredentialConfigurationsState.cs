// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Website.Stores.CredentialIssuer;

[FeatureState]
public record CredentialConfigurationsState
{
    public CredentialConfigurationsState()
    {

    }

    public Dictionary<string, CredentialConfiguration> CredentialConfigurations { get; set; } = new Dictionary<string, CredentialConfiguration>();
    public bool IsLoading { get; set; } = true;
}

public class CredentialConfiguration
{
    [JsonPropertyName("format")]
    public string Format { get; set; }
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
    [JsonPropertyName("cryptographic_binding_methods_supported")]
    public List<string> CryptographicBindingMethodsSupported { get; set; }
    [JsonPropertyName("cryptographic_suites_supported")]
    public List<string> CryptographicSuitesSupported { get; set; }
    [JsonPropertyName("proof_types")]
    public List<string> ProofTypes { get; set; }
    public List<CredentialConfigurationDisplay> Displays { get; set; }
}

public class CredentialConfigurationDisplay
{
    [JsonPropertyName("locale")]
    public string Locale { get; set; }
    [JsonPropertyName("logo")]
    public CredentialConfigurationDisplayLogo Logo { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("background_color")]
    public string BackgroundColor { get; set; }
    [JsonPropertyName("text_color")]
    public string TextColor { get; set; }
}

public class CredentialConfigurationDisplayLogo
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; }
    [JsonPropertyName("alt_text")]
    public string AltText { get; set; }
}