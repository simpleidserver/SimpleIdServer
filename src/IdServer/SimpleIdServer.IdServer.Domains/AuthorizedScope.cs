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
        public ICollection<string> Resources
        {
            get
            {
                return AuthorizedResources == null ? new List<string>() : AuthorizedResources.Select(r => r.Resource).ToList();
            }
        }
        [JsonIgnore]
        public ICollection<string> Audiences
        {
            get
            {
                return AuthorizedResources == null ? new List<string>() : AuthorizedResources.Where(r => !string.IsNullOrWhiteSpace(r.Audience)).Select(r => r.Audience).ToList();
            }
        }
        [JsonIgnore]
        public Consent Consent { get; set; }
        [JsonIgnore]
        public ICollection<AuthorizedResource> AuthorizedResources { get; set; }
    }

    public class AuthorizedResource
    {
        [JsonIgnore]
        public string Resource { get; set; } = null!;
        [JsonIgnore]
        public string? Audience { get; set; } = null;
    }
}
