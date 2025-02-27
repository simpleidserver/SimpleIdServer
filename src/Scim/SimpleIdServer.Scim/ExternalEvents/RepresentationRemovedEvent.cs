// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Newtonsoft.Json.Linq;

namespace SimpleIdServer.Scim.ExternalEvents
{
    public class RepresentationRemovedEvent: IntegrationEvent
    {
        public RepresentationRemovedEvent()
        {

        }

        public RepresentationRemovedEvent(string id, string version, string resourceType, string realm, JObject representation, string token) : base(id, version, realm, resourceType, representation) 
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
