// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.DTOs
{
    public class CredentialIssuerDisplayResult
    {
        [JsonPropertyName(CredentialOfferResultNames.Name)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; } = null;
        [JsonPropertyName(CredentialOfferResultNames.Locale)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; } = null;
    }
}
