// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.SubjectTypeBuilders;
using System;

namespace SimpleIdServer.IdServer.UI.ViewModels;

public class SessionViewModel
{
    public SessionViewModel(string name, string realm, DateTimeOffset? expiresUct, DateTimeOffset? issuedUtc)
    {
        Name = name;
        Realm = realm;
        ExpiresUct = expiresUct;
        IssuedUtc = issuedUtc;
    }

    public string Name { get; }
    public string Realm { get; }
    public DateTimeOffset? ExpiresUct { get; }
    public DateTimeOffset? IssuedUtc { get; }
}
