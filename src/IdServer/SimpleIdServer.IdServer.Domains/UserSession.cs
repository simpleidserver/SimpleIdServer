// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class UserSession : ICloneable
    {
        [JsonPropertyName(UserSessionNames.SessionId)]
        public string SessionId { get; set; } = null!;
        [JsonPropertyName(UserSessionNames.AuthenticationDateTime)]
        public DateTime AuthenticationDateTime { get; set; }
        [JsonPropertyName(UserSessionNames.ExpirationDateTime)]
        public DateTime ExpirationDateTime { get; set; }
        [JsonPropertyName(UserSessionNames.State)]
        public UserSessionStates State { get; set; }
        [JsonPropertyName(UserSessionNames.Realm)]
        public string Realm { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public bool IsActive() => State == UserSessionStates.Active && DateTime.UtcNow <= ExpirationDateTime;

        public object Clone()
        {
            return new UserSession
            {
                SessionId = SessionId,
                AuthenticationDateTime = AuthenticationDateTime,
                ExpirationDateTime = ExpirationDateTime,
                State = State
            };
        }
    }
}
