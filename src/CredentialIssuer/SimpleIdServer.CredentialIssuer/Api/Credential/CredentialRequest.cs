// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.Credential
{
    [JsonConverter(typeof(CredentialRequestConverter))]
    public class CredentialRequest
    {
        // format: mso_mdoc
        // doctype : org.iso.18013.5.1.mDL

        // 

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
        /// An object containing a single public key as a JWK used for encrypting the Credential Response.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.CredentialEncryptionJwk)]
        public JsonObject CredentialEncryptionJwk { get; set; }

        /// <summary>
        /// JWE [RFC7516] alg algorithm [RFC7518] REQUIRED for encrypting Credential and/or Batch Credential Responses.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.CredentialResponseEncryptionAlg)]
        public string CredentialResponseEncryptionAlg { get; set; }

        /// <summary>
        /// OPTIONAL. JWE [RFC7516] enc algorithm [RFC7518] REQUIRED for encrypting Credential Responses.
        /// </summary>
        [JsonPropertyName(CredentialRequestNames.CredentialResponseEncryptionEnc)]
        public string CredentialResponseEncryptionEnc { get; set; }

        [JsonIgnore]
        public JsonObject Data { get; set; }
    }
}
