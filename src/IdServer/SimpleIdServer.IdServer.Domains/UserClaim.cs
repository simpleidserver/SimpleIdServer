// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class UserClaim
    {
        public UserClaim() { }

        public UserClaim(string id, string name, string value)
        {
            Name = name;
            Value = value;
        }

        public UserClaim(string id, string name, string value, string type) : this(id, name, value)
        {
            Type = type;
        }

        [JsonPropertyName(UserClaimNames.Id)]
        public string Id { get; set; } = null!;

        [JsonPropertyName(UserClaimNames.Name)]
        public string Name { get; set; } = null!;

        [JsonPropertyName(UserClaimNames.Value)]
        public string Value { get; set; } = null!;

        [JsonPropertyName(UserClaimNames.Type)]
        public string? Type { get; set; } = null;
        [JsonIgnore]
        public string? UserId { get; set; } = null;
        [JsonIgnore]
        public User User { get; set; }
    }
}
