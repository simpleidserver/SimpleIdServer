// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Scim.DTOs
{
    public class GetSCIMResourceRequest
    {
        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to return in the response.
        /// </summary>
        [JsonPropertyName(SCIMConstants.StandardSCIMSearchAttributes.Attributes)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.Attributes)]
        public IEnumerable<string> Attributes { get; set; } = new List<string>();
        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to be removed from the default set of attributes to return.
        /// </summary>
        [JsonPropertyName(SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes)]
        public IEnumerable<string> ExcludedAttributes { get; set; } = new List<string>();
    }
}
