// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Helpers;

public interface IRealmStore
{
    string Realm { get; set; }
}

public class RealmStore : IRealmStore
{
    public string Realm { get; set; }
}