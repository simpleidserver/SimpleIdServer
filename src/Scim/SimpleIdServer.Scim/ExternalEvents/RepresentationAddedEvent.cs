// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Common.ExternalEvents;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationAddedEvent : IntegrationEvent
    {
        public RepresentationAddedEvent(string id, int version, string resourceType, JObject representation) : base(id, version, resourceType, representation) { }
    }
}
