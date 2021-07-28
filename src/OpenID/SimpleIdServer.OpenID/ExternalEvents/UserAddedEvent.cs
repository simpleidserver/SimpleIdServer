// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.ExternalEvents
{
    public class UserAddedEvent
    {
        public UserAddedEvent(string userId, Dictionary<string, string> claims)
        {
            UserId = userId;
            Claims = claims;
        }

        public string UserId { get; set; }
        public Dictionary<string, string> Claims { get; set; }
    }
}
