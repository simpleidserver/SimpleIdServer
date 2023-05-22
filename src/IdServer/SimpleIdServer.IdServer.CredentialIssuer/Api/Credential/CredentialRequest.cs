// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.Credential
{
    [JsonConverter(typeof(CredentialRequestConverter))]
    public class CredentialRequest
    {
        /// <summary>
        /// Format of the Credential to be issued.
        /// The Credential format identifier determines further parameters required to determine the type and (optionally) the content of the credential to be issued.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.Format)]
        public string Format { get; set; }
        /// <summary>
        /// JSON object containing proof of possession of the key material the issued Credential shall be bound to.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.Proof)]
        public CredentialProofRequest Proof { get; set; }
        [JsonIgnore]
        public JsonObject OtherParameters { get; set; } = new JsonObject();
    }
}
