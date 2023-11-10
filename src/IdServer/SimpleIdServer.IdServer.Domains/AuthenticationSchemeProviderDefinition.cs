// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class AuthenticationSchemeProviderDefinition
    {
        [JsonPropertyName(AuthenticationSchemeProviderDefinitionNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(AuthenticationSchemeProviderDefinitionNames.Description)]
        public string? Description { get; set; } = null;
        [JsonPropertyName(AuthenticationSchemeProviderDefinitionNames.Image)]
        public string? Image { get; set; } = null;
        [JsonPropertyName(AuthenticationSchemeProviderDefinitionNames.HandlerFullQualifiedName)]
        public string? HandlerFullQualifiedName { get; set; } = null;
        [JsonPropertyName(AuthenticationSchemeProviderDefinitionNames.OptionsFullQualifiedName)]
        public string? OptionsFullQualifiedName { get; set; } = null;
        [JsonPropertyName(AuthenticationSchemeProviderDefinitionNames.OptionsName)]
        public string? OptionsName { get; set; } = null;
        [JsonIgnore]
        public ICollection<AuthenticationSchemeProvider> AuthSchemeProviders { get; set; } = new List<AuthenticationSchemeProvider>();
    }
}
