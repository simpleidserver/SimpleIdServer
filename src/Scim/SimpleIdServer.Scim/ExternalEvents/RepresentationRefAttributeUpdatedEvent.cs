// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationRefAttributeUpdatedEvent : BaseReferenceAttributeEvent
    {
        public RepresentationRefAttributeUpdatedEvent()
        {
        }

        public RepresentationRefAttributeUpdatedEvent(SCIMRepresentationAttribute attr) : base(attr)
        {
        }
    }
}
