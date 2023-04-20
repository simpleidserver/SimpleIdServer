// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Did.Models
{
    public class IdentityDocumentVerificationMethod
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        /// <summary>
        /// Entity that is authorized to make changes to a DID document.
        /// The process of authorizing a DID controller is defined by the DID method.
        /// </summary>
        [JsonPropertyName("controller")]
        public string Controller { get; set; }
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
        [JsonPropertyName("publicKeyJwk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonObject PublicKeyJwk { get; set; } = null;
        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Value { get; set; } = null;
    }
}