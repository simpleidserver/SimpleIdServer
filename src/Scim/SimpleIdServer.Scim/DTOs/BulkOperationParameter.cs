// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using System.Runtime.Serialization;

namespace SimpleIdServer.Scim.DTOs
{
    public class BulkOperationParameter
    {
        /// <summary>
        /// The HTTP method of the current operation.  Possible values are "POST", "PUT", "PATCH", or "DELETE".  REQUIRED.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Method)]
        [JsonProperty(StandardSCIMRepresentationAttributes.Method)]
        public string HttpMethod { get; set; }
        /// <summary>
        ///  The transient identifier of a newly created resource, unique within a bulk request and created by the client.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.BulkId)]
        [JsonProperty(StandardSCIMRepresentationAttributes.BulkId)]
        public string BulkIdentifier { get; set; }
        /// <summary>
        /// The current resource version.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Version)]
        [JsonProperty(StandardSCIMRepresentationAttributes.Version)]
        public string Version { get; set; }
        /// <summary>
        /// The resource's relative path to the SCIM service provider's root.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Path)]
        [JsonProperty(StandardSCIMRepresentationAttributes.Path)]
        public string Path { get; set; }
        /// <summary>
        /// The resource data as it would appear for a single SCIM POST, PUT, or PATCH operation.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Data)]
        [JsonProperty(StandardSCIMRepresentationAttributes.Data)]
        public JToken Data { get; set; }
    }
}
