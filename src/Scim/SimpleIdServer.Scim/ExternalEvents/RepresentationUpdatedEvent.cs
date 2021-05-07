// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationUpdatedEvent : IntegrationEvent
    {
        public RepresentationUpdatedEvent(string id, string resourceType, JObject representation) : base(id, resourceType, representation) { }
    }
}
