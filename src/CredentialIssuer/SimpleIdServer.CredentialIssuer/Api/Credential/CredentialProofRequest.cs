// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.CredentialIssuer.Converters;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    [JsonConverter(typeof(OtherDataConverter))]
    public class CredentialProofRequest
    {
        /// <summary>
        /// Concrete proof type.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.ProofType)]
        public string ProofType { get; set; }

        [JsonIgnore]
        public JsonObject Data { get; set; }
    }
}
