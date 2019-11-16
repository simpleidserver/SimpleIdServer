// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.Scim.DTOs
{
    public class SCIMBulkOperationRequest
    {
        public SCIMBulkOperationRequest()
        {

        }

        public string HttpMethod { get; set; }
        public string BulkIdentifier { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
        public JToken Data { get; set; }
    }
}
