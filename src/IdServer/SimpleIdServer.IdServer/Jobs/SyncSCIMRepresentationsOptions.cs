// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Serializer;

namespace SimpleIdServer.IdServer.Jobs
{
    public class SyncSCIMRepresentationsOptions
    {
        [VisibleProperty("SCIMEdp", "SCIM Endpoint")]
        public string SCIMEdp { get; set; }
        [VisibleProperty("ClientId", "Client Identifier")]
        public string ClientId { get; set; }
        [VisibleProperty("ClientSecret", "Client Secret")]
        public string ClientSecret { get; set; }
        [VisibleProperty("Count", "Maximum number of records returned by the SCIM endpoint")]
        public int Count { get; set; }
    }
}
