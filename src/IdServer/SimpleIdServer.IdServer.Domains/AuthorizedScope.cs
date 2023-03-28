// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class AuthorizedScope
    {
        [JsonPropertyName(GrantParameters.Scope)]
        public string Scope { get; set; } = null!;
        [JsonPropertyName(GrantParameters.Resources)]
        public ICollection<string> Resources { get; set; } = new List<string>();
        [JsonIgnore]
        public Consent Consent { get; set; }
    }
}
