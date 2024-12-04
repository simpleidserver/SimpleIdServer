// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.DTOs;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationUpdatedEvent : IntegrationEvent
    {
        public RepresentationUpdatedEvent()
        {

        }

        public RepresentationUpdatedEvent(string id, string version, string resourceType, JObject representation, string token, List<SCIMPatchResult> patchOperations) : base(id, version, resourceType, representation) 
        {
            Token = token;
            PatchOperations = PatchAttributeOperation.Transform(patchOperations);
        }

        public string Token { get; set; }
        public List<PatchAttributeOperation> PatchOperations { get; set; }
    }
}
