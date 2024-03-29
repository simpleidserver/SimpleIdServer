﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.UI.Services
{
    public interface IIdProviderAuthService
    {
        string Name { get; }
        bool Authenticate(User user, IdentityProvisioning identityProvisioning, string password);
    }
}
