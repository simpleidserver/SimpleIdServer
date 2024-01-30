// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    public class CredentialProofRequest
    {
        /// <summary>
        /// Concrete proof type.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.ProofType)]
        public string ProofType { get; set; }

        [JsonPropertyName(CredentialRequestNames.Jwt)]
        public string Jwt { get; set; }
    }
}
