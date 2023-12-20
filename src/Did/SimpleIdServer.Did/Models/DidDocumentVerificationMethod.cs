// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static SimpleIdServer.Did.Constants;

namespace SimpleIdServer.Did.Models
{
    // [JsonConverter(typeof(IdentityDocumentVerificationMethodConverter))]
    public class DidDocumentVerificationMethod
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// MUST be a string that references exactly one verification method type.
        /// The verification method must be registered in the DID specification registries (https://www.w3.org/TR/did-spec-registries/).
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
        /// <summary>
        /// Entity that is authorized to make changes to a DID document.
        /// The process of authorizing a DID controller is defined by the DID method.
        /// </summary>
        [JsonPropertyName("controller")]
        public string Controller { get; set; }
        // https://www.w3.org/TR/did-spec-registries/#verification-method-properties
        /// <summary>
        /// JSON Web Key.
        /// MUST NOT contain "d" or any other members of the private information.
        /// </summary>
        [JsonIgnore]
        public JsonWebKey PublicKeyJwk { get; set; } = null;
        [JsonPropertyName("publicKeyJwk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonObject SerializedPublicKeyJwk
        {
            get
            {
                if (PublicKeyJwk == null) return null;
                var json = JsonExtensions.SerializeToJson(PublicKeyJwk);
                return JsonObject.Parse(json).AsObject();
            }
        }

        /// <summary>
        /// MULTIBASE encoded public key.
        /// </summary>
        [JsonPropertyName("publicKeyMultibase")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicKeyMultibase { get; set; } = null;


        [JsonPropertyName("blockchainAccountId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BlockChainAccountId { get; set; }
        [JsonPropertyName("publicKeyHex")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicKeyHex { get; set; } = null;
        [JsonPropertyName("publicKeyBase64")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicKeyBase64 { get; set; } = null;
        [JsonPropertyName("publicKeyBase58")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicKeyBase58 { get; set; } = null;
        [JsonPropertyName("publicKeyPem")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicKeyPem { get; set; } = null;
        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Value { get; set; } = null;
        [JsonIgnore]
        public VerificationMethodUsages Usage { get; set; }
        [JsonIgnore]
        public Dictionary<string, string> AdditionalParameters = new Dictionary<string, string>();
    }
}