// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationReferenceAttributeAddedEvent : BaseReferenceAttributeEvent
    {
        public RepresentationReferenceAttributeAddedEvent()
        {

        }

        public RepresentationReferenceAttributeAddedEvent(string id, int version, string resourceType, string representationAggregateId, string schemaAttributeId, string attributeFullPath, JObject representation) : base(id, version, resourceType, representationAggregateId, schemaAttributeId, attributeFullPath, representation)
        {
        }
    }
}
