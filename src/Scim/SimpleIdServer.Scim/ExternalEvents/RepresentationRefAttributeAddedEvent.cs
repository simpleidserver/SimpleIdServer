// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationRefAttributeAddedEvent : BaseReferenceAttributeEvent
    {
        public RepresentationRefAttributeAddedEvent()
        {

        }

        public RepresentationRefAttributeAddedEvent(
            string representationId,
            string resourceType,
            string fromRepresentationId,
            string fromRepresentationType,
            string? realm) : base(representationId, resourceType, fromRepresentationId, fromRepresentationType, representationId)
        {
        }
    }
}
