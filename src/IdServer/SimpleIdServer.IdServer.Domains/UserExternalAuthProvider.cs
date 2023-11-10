// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class UserExternalAuthProvider : ICloneable
    {
        [JsonPropertyName(UserExternalAuthProviderNames.Scheme)]
        public string Scheme { get; set; } = null!;
        [JsonPropertyName(UserExternalAuthProviderNames.Subject)]
        public string Subject { get; set; } = null!;
        [JsonPropertyName(UserExternalAuthProviderNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public object Clone()
        {
            return new UserExternalAuthProvider
            {
                Scheme = Scheme,
                Subject = Subject,
                CreateDateTime = CreateDateTime
            };
        }
    }
}
