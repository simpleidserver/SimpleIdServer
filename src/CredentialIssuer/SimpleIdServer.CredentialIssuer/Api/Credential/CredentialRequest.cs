// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.CredentialIssuer.Converters;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    [JsonConverter(typeof(OtherDataJsonConverter<CredentialRequest>))]
    public class CredentialRequest : IOtherData
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

        /// <summary>
        /// REQUIRED when credential_identifier was returned from the Token Response. MUST NOT be used otherwise.
        /// String that identifies a Credential that is being requested to be issued.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.CredentialIdentifier)]
        public string Credentialidentifier { get; set; }

        /// <summary>
        /// Object containing information for encrypting the Credential Response.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.CredentialResponseEncryption)]
        public CredentialResponseEncryption CredentialResponseEncryption { get; set; }

        [JsonIgnore]
        public JsonObject Data { get; set; }
    }

    public class CredentialResponseEncryption
    {
        [JsonPropertyName(CredentialRequestNames.Jwk)]
        public JsonWebKey Jwk { get; set; }
        [JsonPropertyName(CredentialRequestNames.Alg)]
        public string Alg { get; set; }
        [JsonPropertyName(CredentialRequestNames.Enc)]
        public string Enc { get; set; }
    }
}
