// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace SimpleIdServer.Scim.DTOs
{
    public class PatchOperationParameter
    {
        /// <summary>
        /// Indicates the operation to perform and MAY be one of "add", "remove", or "replace".
        /// </summary>
        [DataMember(Name = SCIMConstants.PathOperationAttributes.Operation)]
        [JsonProperty(SCIMConstants.PathOperationAttributes.Operation)]
        public SCIMPatchOperations Operation { get; set; }
        /// <summary>
        /// Attribute path describing the target of the operation.
        /// </summary>
        [DataMember(Name = SCIMConstants.PathOperationAttributes.Path)]
        [JsonProperty(SCIMConstants.PathOperationAttributes.Path)]
        public string Path { get; set; }
        /// <summary>
        /// "value" member whose content specifies the value to be added / removed / replaced.
        /// </summary>
        [DataMember(Name = SCIMConstants.PathOperationAttributes.Value)]
        [JsonProperty(SCIMConstants.PathOperationAttributes.Value)]
        public object Value { get; set; }
    }
}
