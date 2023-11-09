// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models
{
    public class CredentialTemplateParameter
    {
        [JsonPropertyName(CredentialParameterNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(CredentialParameterNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(CredentialParameterNames.Value)]
        public string Value { get; set; } = null!;
        [JsonIgnore]
        public string CredentialTemplateId { get; set; } = null!;
        [JsonIgnore]
        public BaseCredentialTemplate CredentialTemplate { get; set; } = null!;
        [JsonPropertyName(CredentialParameterNames.Type)]
        public CredentialTemplateParameterTypes ParameterType { get; set; } = CredentialTemplateParameterTypes.STRING;
        [JsonPropertyName(CredentialParameterNames.IsArray)]
        public bool IsArray { get; set; } = false;
    }

    public enum CredentialTemplateParameterTypes
    {
        STRING = 0,
        JSON = 1
    }
}
