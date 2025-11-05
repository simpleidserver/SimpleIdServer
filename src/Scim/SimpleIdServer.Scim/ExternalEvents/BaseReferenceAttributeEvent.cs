// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class BaseReferenceAttributeEvent : IntegrationEvent
    {
        public BaseReferenceAttributeEvent()
        {

        }

        public BaseReferenceAttributeEvent(
            string representationId,
            string resourceType,
            string fromRepresentationId,
            string fromRepresentationType,
            string? realm)
        {
            RepresentationId = representationId;
            ResourceType = resourceType;
            FromRepresentationId = fromRepresentationId;
            FromRepresentationType = fromRepresentationType;
            Realm = realm;
        }

        public string RepresentationId { get; set; }

        public string ResourceType {  get; set; }

        public string FromRepresentationId { get; set; }

        public string FromRepresentationType { get; set; }

        public string? Realm { get; set; }
    }
}
