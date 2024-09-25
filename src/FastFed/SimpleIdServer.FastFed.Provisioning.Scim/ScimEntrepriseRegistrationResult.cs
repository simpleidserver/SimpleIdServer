// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.FastFed.Provisioning.Scim;

public class ScimEntrepriseRegistrationResult
{
    [JsonPropertyName("scim_service_uri")]
    public string ScimServiceUri { get; set; }
    [JsonPropertyName("provider_authentication_methods")]
    public string ProviderAuthenticationMethods { get; set; }
    [JsonPropertyName(Constants.JwtProfile)]
    public AuthenticationProfileResult JwtProfile { get; set; }
}