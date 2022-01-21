// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json;
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleIdServer.Scim.DTOs
{
    public class BulkParameter : BaseParameter
    {
        public BulkParameter()
        {
            Operations = new List<BulkOperationParameter>();
        }

        /// <summary>
        /// Defines operations within a bulk job.
        /// </summary>
        [DataMember(Name = StandardSCIMRepresentationAttributes.Operations)]
        [JsonProperty(StandardSCIMRepresentationAttributes.Operations)]
        public ICollection<BulkOperationParameter> Operations { get; set; }
    }
}
