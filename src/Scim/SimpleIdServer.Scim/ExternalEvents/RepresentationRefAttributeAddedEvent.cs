// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Domains;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationRefAttributeAddedEvent : BaseReferenceAttributeEvent
    {
        public RepresentationRefAttributeAddedEvent()
        {

        }

        public RepresentationRefAttributeAddedEvent(SCIMRepresentationAttribute attr) : base(attr)
        {
        }
    }
}
