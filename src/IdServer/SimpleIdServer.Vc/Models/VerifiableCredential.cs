// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models
{
    public class VerifiableCredential
    {
        [JsonPropertyName("@context")]
        public ICollection<string> Context { get; set; } = new List<string>();
        /// <summary>
        /// The value of the type property MUST be, or map to (through interpretation of the @context property), one or more URIs. 
        /// </summary>
        [JsonPropertyName("type")]
        public ICollection<string> Type { get; set; } = new List<string>();
        /// <summary>
        /// Set of objects that contain one or more properties that each related to a subject of the verifiable credential.
        /// Each object may contain an id.
        /// </summary>
        [JsonPropertyName("credentialSubject")]
        public JsonObject CredentialSubject { get; set; }
        [JsonPropertyName("issuer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Issuer { get; set; } = null;
        [JsonPropertyName("issuanceDate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? IssuanceDate { get; set; } = null;

        public string Serialize() => JsonSerializer.Serialize(this);        

        public Dictionary<string, object> SerializeToDic()
        {
            var jObj = JsonObject.Parse(JsonSerializer.Serialize(this)).AsObject();
            return jObj.Serialize() as Dictionary<string, object>;
        }
    }
}
