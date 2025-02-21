// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Helpers;

namespace SimpleIdServer.IdServer.Infastructures;

public class RealmStore : IRealmStore
{
    public string Realm { get; set; }
}
