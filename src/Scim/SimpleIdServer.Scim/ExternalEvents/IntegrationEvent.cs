// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class IntegrationEvent
    {
        public IntegrationEvent(string id, string resourceType)
        {
            Id = id;
            ResourceType = resourceType;
        }


        public IntegrationEvent(string id, string resourceType, JObject representation) : this(id, resourceType)
        {
            Representation = representation;
        }

        public string Id { get; set; }
        public string ResourceType { get; set; }
        public JObject Representation { get; set; }
    }
}
