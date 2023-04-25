// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models
{
    /// <summary>
    /// MAY be used to combine and present credentials.
    /// They can be packaged in such a way that the authorship of the data is verifiable.
    /// </summary>
    public class VerifiablePresentation
    {
        [JsonPropertyName("@context")]
        public ICollection<string> Context { get; set; } = new List<string>();
        /// <summary>
        /// OPTIONAL Unique identifier for the presentation.
        /// </summary>
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; } = null;
        /// <summary>
        /// REQUIRED and expresses the type of presentation such as VerifiablePresentation.
        /// </summary>
        [JsonPropertyName("type")]
        public ICollection<string> Type { get; set; } = new List<string>();
        [JsonPropertyName("verifiableCredential")]
        public ICollection<string> VerifiableCredential { get; set; } = new List<string>();
        [JsonIgnore]
        public ICollection<VerifiableCredential> VerifiableCredentials { get; set; } = new List<VerifiableCredential>();

        public Dictionary<string, object> SerializeToDic()
        {
            var jObj = JsonObject.Parse(JsonSerializer.Serialize(this)).AsObject();
            return jObj.Serialize() as Dictionary<string, object>;
        }
    }
}
