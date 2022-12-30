// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.DTOs
{
    public class GetSCIMResourceParameter
    {
        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to return in the response.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.Attributes)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.Attributes)]
        public IEnumerable<string> Attributes { get; set; } = new List<string>();
        /// <summary>
        /// A multi-valued list of strings indicating the names of resource attributes to be removed from the default set of attributes to return.
        /// </summary>
        [JsonProperty(SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes)]
        [FromQuery(Name = SCIMConstants.StandardSCIMSearchAttributes.ExcludedAttributes)]
        public IEnumerable<string> ExcludedAttributes { get; set; } = new List<string>();
    }
}
