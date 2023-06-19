// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Vc.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models
{
    public class W3CCredentialSubject
    {
        /// <summary>
        /// OPTIONAL. Boolean which when set to true indicates the claim MUST be present in the issued Credential.
        /// </summary>
        [JsonPropertyName(W3CCredentialSubjectNames.Mandatory)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Mandatory { get; set; }
        /// <summary>
        /// OPTIONAL. String value determining type of value of the claim.
        /// </summary>
        [JsonPropertyName(W3CCredentialSubjectNames.ValueType)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ValueType { get; set; } = null;
        /// <summary>
        ///  OPTIONAL. An array of objects, where each object contains display properties of a certain claim in the Credential for a certain language
        /// </summary>
        [JsonPropertyName(W3CCredentialSubjectNames.Display)]
        public ICollection<W3CCredentialSubjectDisplay> Display { get; set; } = new List<W3CCredentialSubjectDisplay>();
    }

    public class W3CCredentialSubjectDisplay
    {
        /// <summary>
        /// OPTIONAL : String value of a display name for the claim.: 
        /// </summary>
        [JsonPropertyName(W3CCredentialSubjectNames.Name)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; } = null;
        /// <summary>
        /// OPTIONAL : String value that identifies language of this object represented as language tag values.
        /// </summary>
        [JsonPropertyName(W3CCredentialSubjectNames.Locale)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Locale { get; set; } = null;
    }
}
