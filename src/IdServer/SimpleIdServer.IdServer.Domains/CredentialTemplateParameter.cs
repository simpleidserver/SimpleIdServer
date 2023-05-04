// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class CredentialTemplateParameter
    {
        [JsonIgnore]
        public string Id { get; set; } = null!;
        [JsonIgnore]
        public string Name { get; set; } = null!;
        [JsonIgnore]
        public string Value { get; set; } = null!;
        [JsonIgnore]
        public string JsonPath { get; set; } = null!;
        [JsonIgnore]
        public string CredentialTemplateId { get; set; } = null!;
        [JsonIgnore]
        public CredentialTemplate CredentialTemplate { get; set; } = null!;
    }
}
