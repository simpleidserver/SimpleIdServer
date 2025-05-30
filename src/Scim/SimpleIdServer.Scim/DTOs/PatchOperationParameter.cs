// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Infrastructure.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.DTOs
{
    [JsonConverter(typeof(PatchOperationParameterConverter))]
    public class PatchOperationParameter
    {
        /// <summary>
        /// Indicates the operation to perform and MAY be one of "add", "remove", or "replace".
        /// </summary>
        [DataMember(Name = SCIMConstants.PathOperationAttributes.Operation)]
        [JsonPropertyName(SCIMConstants.PathOperationAttributes.Operation)]
        public SCIMPatchOperations? Operation { get; set; }
        /// <summary>
        /// Attribute path describing the target of the operation.
        /// </summary>
        [DataMember(Name = SCIMConstants.PathOperationAttributes.Path)]
        [JsonPropertyName(SCIMConstants.PathOperationAttributes.Path)]
        public string Path { get; set; }
        /// <summary>
        /// "value" member whose content specifies the value to be added / removed / replaced.
        /// </summary>
        [DataMember(Name = SCIMConstants.PathOperationAttributes.Value)]
        [JsonPropertyName(SCIMConstants.PathOperationAttributes.Value)]
        public JsonNode Value { get; set; }
    }
}
