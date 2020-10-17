// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleIdServer.Scim.DTOs
{
    public class PatchRepresentationParameter : BaseParameter
    {
        /// <summary>
        /// Is an array of one or more PATCH operations.
        /// </summary>
        [DataMember(Name = SCIMConstants.PathOperationAttributes.Operations)]
        [JsonProperty(SCIMConstants.PathOperationAttributes.Operations)]
        public ICollection<PatchOperationParameter> Operations { get; set; }
    }
}
