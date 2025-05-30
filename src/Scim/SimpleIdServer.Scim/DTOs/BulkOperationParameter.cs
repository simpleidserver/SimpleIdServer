// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Runtime.Serialization;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.DTOs
{
    public class BulkOperationParameter
    {
        /// <summary>
        /// The HTTP method of the current operation.  Possible values are "POST", "PUT", "PATCH", or "DELETE".  REQUIRED.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Method)]
        [JsonPropertyName(StandardSCIMRepresentationAttributes.Method)]
        public string HttpMethod { get; set; }
        /// <summary>
        ///  The transient identifier of a newly created resource, unique within a bulk request and created by the client.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.BulkId)]
        [JsonPropertyName(StandardSCIMRepresentationAttributes.BulkId)]
        public string BulkIdentifier { get; set; }
        /// <summary>
        /// The current resource version.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Version)]
        [JsonPropertyName(StandardSCIMRepresentationAttributes.Version)]
        public string Version { get; set; }
        /// <summary>
        /// The resource's relative path to the SCIM service provider's root.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Path)]
        [JsonPropertyName(StandardSCIMRepresentationAttributes.Path)]
        public string Path { get; set; }
        /// <summary>
        /// The resource data as it would appear for a single SCIM POST, PUT, or PATCH operation.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Data)]
        [JsonPropertyName(StandardSCIMRepresentationAttributes.Data)]
        public JsonNode Data { get; set; }
    }
}
