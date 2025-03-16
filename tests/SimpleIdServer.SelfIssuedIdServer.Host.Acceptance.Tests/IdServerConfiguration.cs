// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.SelfIdServer.Host.Acceptance.Tests;

public class IdServerConfiguration
{
    public static List<SimpleIdServer.IdServer.Domains.Realm> Realms = new List<SimpleIdServer.IdServer.Domains.Realm>
    {
        SimpleIdServer.IdServer.Config.DefaultRealms.Master
    };
}