// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultAcrs
{
    public static List<string> AllNames => new List<string>
    {
        FirstLevelAssurance.Name
    };

    public static AuthenticationContextClassReference FirstLevelAssurance = new AuthenticationContextClassReference
    {
        Id = Guid.NewGuid().ToString(),
        Name = "sid-load-01",
        DisplayName = "First level of assurance",
        UpdateDateTime = DateTime.UtcNow,
        Realms = new List<Realm>
            {
                Config.DefaultRealms.Master
            }
    };
}
