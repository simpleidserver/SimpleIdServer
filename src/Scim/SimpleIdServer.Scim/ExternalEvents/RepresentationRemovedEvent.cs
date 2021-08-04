// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.ExternalEvents;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationRemovedEvent: IntegrationEvent
    {
        public RepresentationRemovedEvent(string id, int version, string resourceType) : base(id, version, resourceType) { }
    }
}
