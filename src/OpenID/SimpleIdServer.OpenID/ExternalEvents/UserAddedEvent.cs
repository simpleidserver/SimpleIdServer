// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Common.ExternalEvents;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.ExternalEvents
{
    public class UserAddedEvent: IntegrationEvent
    {
        public UserAddedEvent(string id, int version, string resourceType, Dictionary<string, string> claims) : base(id, version, resourceType)
        {
            if (claims != null)
            {
                Representation = JObject.FromObject(claims);
            }
        }
    }
}
