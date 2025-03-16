// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultRealms
{
    public static Domains.Realm Master = new Domains.Realm
    {
        Name = Constants.DefaultRealm,
        CreateDateTime = DateTime.UtcNow,
        UpdateDateTime = DateTime.UtcNow,
        Description = Constants.DefaultRealm
    };
}
