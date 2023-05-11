// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.Vc.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.CredentialIssuer
{
    public class CredentialIssuerResult
    {
        /// <summary>
        /// Identifier of the OAUTH2.0 Authorization server, the credential issuer relies on for the authorization.
        /// If this element is omitted, the entity providing the credential issuer is also acting as the AS.
        /// </summary>
        [JsonPropertyName(CredentialIssuerResultNames.AuthorizationServer)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationServer { get; set; } = null;
        [JsonPropertyName(CredentialIssuerResultNames.CredentialIssuer)]
        public string CredentialIssuer { get; set; } = null!;
        [JsonPropertyName(CredentialIssuerResultNames.CredentialEndpoint)]
        public string CredentialEndpoint { get; set; } = null!;
        /// <summary>
        /// List of JSON Objects, each of them representing metadata about a separate credential type that the credential issuer can issue.
        /// </summary>
        [JsonPropertyName(CredentialIssuerResultNames.CredentialsSupported)]
        public ICollection<BaseCredentialTemplate> CredentialsSupported { get; set; } = new List<BaseCredentialTemplate>();
        /// <summary>
        /// Each object contains display properties of a Credential Issuer for a certain language.
        /// </summary>
        [JsonPropertyName(CredentialIssuerResultNames.Display)]
        public ICollection<CredentialIssuerDisplayResult> Display { get; set; } = new List<CredentialIssuerDisplayResult>();
    }

    public class CredentialIssuerDisplayResult
    {
        [JsonPropertyName(CredentialIssuerResultNames.Name)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; } = null;
        [JsonPropertyName(CredentialIssuerResultNames.Locale)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; } = null;
    }
}
