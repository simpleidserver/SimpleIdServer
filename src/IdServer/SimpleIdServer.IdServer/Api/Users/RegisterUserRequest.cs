// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.Users
{
    public class RegisterUserRequest
    {
        /// <summary>
        /// Name.
        /// </summary>
        [JsonPropertyName(UserNames.Name)]
        public string Name { get; set; }
        /// <summary>
        /// Firstname.
        /// </summary>
        [JsonPropertyName(UserNames.Firstname)]
        public string Firstname { get; set; }
        /// <summary>
        /// Lastname.
        /// </summary>
        [JsonPropertyName(UserNames.Lastname)]
        public string Lastname { get; set; }
        /// <summary>
        /// Email.
        /// </summary>
        [JsonPropertyName(UserNames.Email)]
        public string? Email { get; set; } = null;
        /// <summary>
        /// Is email verified.
        /// </summary>
        [JsonPropertyName(UserNames.EmailVerified)]
        public bool EmailVerified { get; set; }
        /// <summary>
        /// Claims.
        /// </summary>
        [JsonPropertyName(UserNames.Claims)]
        public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
    }
}